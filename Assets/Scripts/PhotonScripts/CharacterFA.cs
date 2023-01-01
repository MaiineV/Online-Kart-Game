using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class CharacterFA : MonoBehaviourPunCallbacks, IPunObservable
{
    public Player owner;

    public int laps;
    public int waypoint;

    public Transform lastWayPoint;

    [SerializeField] float _maxLife;
    float _currentLife;

    public GameObject myNitro;
    public PhotonView myPhotonView;

    [Header("Variables de Movimiento")]
    public float aceleration;
    public float deaceleration;
    public float maxSpeed;
    public float speed;
    public float turboSpeed;
    public float turboDuration;
    public float maxTurboDuration;
    public bool isTurbo = false;
    public float maxRotationAngle;
    public float rotationAngle;

    [Header("Punto de rotacion en Y")]
    public Transform rotationPoint;

    [Header("Colisiones de Piso")]
    public LayerMask floor;
    public LayerMask collisions;
    public bool isOnAir = true;

    [Header("Variables de fuerzas Verticales")]
    public float verticalForce;
    public float gravity;
    public float forceJump;

    [SerializeField] float _dmg;

    //[SerializeField] NormalBullet _bulletPrefab;
    //[SerializeField] Transform _bulletSpawnPosition;

    public event Action<float> onLifeBarUpdate = delegate { };
    public event Action onDestroy = delegate { };

    void Awake()
    {
        //GetComponent<Renderer>().material.color = Color.red;

        Debug.Log("AWAKE");
    }

    void Start()
    {
        PlayersVar.instance.AddPlayerGameObject(owner, gameObject);
        PlayersVar.instance.AddPlayer(owner);
        Debug.Log("START");
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Gravity();
        ConsumeOfNitro();
    }


    public CharacterFA SetInitialParameters(Player player)
    {
        owner = player;

        _currentLife = _maxLife;

        //GetComponent<Renderer>().material.color = Color.yellow;

        photonView.RPC("SetLocalParms", owner, _currentLife);

        Debug.Log("INITIAL PARAMETERS");

        return this;
    }

    public void TakeDmg(float dmg)
    {
        _currentLife -= dmg;

        onLifeBarUpdate(_currentLife / _maxLife);

        if (_currentLife <= 0)
        {
            photonView.RPC("DisconnectOwner", owner);
            MyServer.Instance.RPC_PlayerDisconnect(owner);
        }
    }

    [PunRPC]
    void DisconnectOwner()
    {
        Debug.LogWarning("SE DESCONECTO");
        PhotonNetwork.Disconnect();
    }

    [PunRPC]
    void SetLocalParms(float life)
    {
        owner = PhotonNetwork.LocalPlayer;

        //GetComponent<Renderer>().material.color = Color.blue;

        Debug.Log("LOCAL PARAMETERS");
    }

    public void Move(float horizontal, float vertical)
    {

        if (vertical > 0)
        {
            if (speed >= 0)
            {
                speed += aceleration * Time.deltaTime;
            }
            else
            {
                speed += deaceleration * Time.deltaTime;
            }
        }
        else if (vertical < 0)
        {
            if (speed <= 0)
            {
                speed -= aceleration * Time.deltaTime;
            }
            else
            {
                speed -= deaceleration * Time.deltaTime;
            }
        }
        else
        {
            speed += (0 - speed) * deaceleration * Time.deltaTime;

            if (0 - speed < 0.1f)
                speed = 0;
        }

        if (!isTurbo)
            speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);
        else
            speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed + turboSpeed);



        if (horizontal > 0.2 || horizontal < -0.2)
        {
            if (speed >= 0)
                rotationAngle = maxRotationAngle * horizontal * Mathf.Abs(speed) / maxSpeed;
            else if (speed < 0)
                rotationAngle = maxRotationAngle * horizontal * -1 * Mathf.Abs(speed) / maxSpeed;
        }
        else
        {
            rotationAngle = 0;
        }

        rotationPoint.Rotate(new Vector3(0, rotationAngle, 0));
        transform.position += rotationPoint.forward * speed * Time.deltaTime;
    }

    public void Jump()
    {
        if (!isOnAir)
        {
            verticalForce += forceJump;
        }
    }

    public void TurboOn()
    {
        if (!isTurbo)
        {
            speed += turboSpeed;
            isTurbo = true;
            myPhotonView.RPC("ActivateVisualNitro", RpcTarget.All);
        }
    }

    public void TurboOff()
    {
        if (isTurbo)
        {
            speed -= turboSpeed;
            isTurbo = false;
            myPhotonView.RPC("DeactivateVisualNitro", RpcTarget.All);
        }
    }

    [PunRPC]
    void ActivateVisualNitro()
    {
        myNitro.gameObject.SetActive(true);
    }

    [PunRPC]
    void DeactivateVisualNitro()
    {
        myNitro.gameObject.SetActive(false);
    }

    public void Gravity()
    {
        RaycastHit hit;
        if (isOnAir && Physics.Raycast(transform.position, Vector3.down, 1.5f, floor))
        {
            isOnAir = false;
            verticalForce = 0;

        }
        else if (!isOnAir && !Physics.Raycast(transform.position, Vector3.down, 1.5f, floor))
        {
            isOnAir = true;
        }

        if (isOnAir)
        {
            verticalForce += gravity * Time.deltaTime;
        }

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f, floor))
        {
            transform.rotation = Quaternion.Euler(new Vector3(hit.collider.transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, hit.collider.transform.rotation.eulerAngles.z));
        }

        transform.position += Vector3.up * verticalForce * Time.deltaTime;
    }

    public void ConsumeOfNitro()
    {
        if (isTurbo)
        {
            turboDuration -= Time.deltaTime;

            if(turboDuration <= 0)
            {
                TurboOff();
            }
        }
        else
        {
            turboDuration += Time.deltaTime;
        }
        turboDuration = Mathf.Clamp(turboDuration, 0, maxTurboDuration);
    }

    private void OnApplicationQuit()
    {
        if (owner == PhotonNetwork.LocalPlayer)
        {
            MyServer.Instance.RequestDisconnection(owner);
        }
        PhotonNetwork.Disconnect();
    }

    private void OnDestroy()
    {
        onDestroy();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_currentLife);
            stream.SendNext(_maxLife);
        }
        else
        {
            _currentLife = (float)stream.ReceiveNext();
            _maxLife = (float)stream.ReceiveNext();
            onLifeBarUpdate(_currentLife / _maxLife);
        }
    }
}
