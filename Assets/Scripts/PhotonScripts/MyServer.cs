using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MyServer : MonoBehaviourPun
{
    public static MyServer Instance;

    Player _server;

    [SerializeField] CharacterFA _characterPrefab;

    Dictionary<Player, CharacterFA> _dictModels = new Dictionary<Player, CharacterFA>();

    public int PackagePerSecond { get; private set; }

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
        {
            if (photonView.IsMine)
            {
                //Este RPC va en direccion a todos los Avatares que se crean
                //cada vez que un cliente nuevo entra a la sala
                photonView.RPC("SetServer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer, 1);
            }
        }
    }

    [PunRPC]
    void SetServer(Player serverPlayer, int sceneIndex = 1)
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _server = serverPlayer;

        PackagePerSecond = 60;

        PhotonNetwork.LoadLevel(sceneIndex);

        var playerLocal = PhotonNetwork.LocalPlayer;

        if (playerLocal != _server)
        {
            //Este RPC lo ejecuta cada servidor avatar en direccion al server original
            photonView.RPC("AddPlayer", _server, playerLocal);
        }

    }

    [PunRPC]
    void AddPlayer(Player player)
    {
        StartCoroutine(WaitForLevel(player));
    }

    IEnumerator WaitForLevel(Player player)
    {
        while(PhotonNetwork.LevelLoadingProgress > 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }

        CharacterFA newCharacter = PhotonNetwork.Instantiate(_characterPrefab.name, Vector3.zero, Quaternion.identity)
                                                .GetComponent<CharacterFA>()
                                                .SetInitialParameters(player);

        _dictModels.Add(player, newCharacter);
    }


    #region REQUESTES QUE RECIBEN LOS SERVIDORES AVATARES

    //Esto lo recibe del Controller y va a llamar por RPC a la funcion Move del host real

    public void RequestMove(Player player, float h, float v)
    {
        photonView.RPC("RPC_Move", _server, player, h, v);
    }

    public void RequestShoot(Player player)
    {
        photonView.RPC("RPC_Shoot", _server, player);
    }

    public void RequestJump(Player player)
    {
        photonView.RPC("RPC_Jump", _server, player);
    }

    public void RequestDisconnection(Player player)
    {
        Debug.LogWarning("ENVIO RPC");

        //PhotonNetwork.SendAllOutgoingCommands();
        photonView.RPC("RPC_PlayerDisconnect", _server, player);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    #endregion

    #region SERVER ORIGINAL
    [PunRPC]
    private void RPC_Move(Player playerRequested, float horizontal, float vertical)
    {
        if (_dictModels.ContainsKey(playerRequested))
        {
            _dictModels[playerRequested].Move(horizontal, vertical);
        }
    }

    [PunRPC]
    void RPC_Shoot(Player playerRequested)
    {
        if (_dictModels.ContainsKey(playerRequested))
        {
            //_dictModels[playerRequested].Shoot();
        }
    }

    [PunRPC]
    void RPC_Jump(Player playerRequested)
    {
        if (_dictModels.ContainsKey(playerRequested))
        {
            _dictModels[playerRequested].Jump();
        }
    }

    [PunRPC]
    public void RPC_PlayerDisconnect(Player player)
    {
        PhotonNetwork.Destroy(_dictModels[player].gameObject);
        _dictModels.Remove(player);
    }
    #endregion

}
