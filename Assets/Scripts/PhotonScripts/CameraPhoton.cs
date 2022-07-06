using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CameraPhoton : MonoBehaviour
{
    Camera _mainCamera;
    Player _owner;
    Transform _followPlayer;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient) return;

        _owner = FindObjectOfType<ControllerFA>().localPlayer;
        _mainCamera = Camera.main;
        _mainCamera.transform.parent = transform;
        _mainCamera.transform.localPosition = new Vector3(0, 0, -30);
        _mainCamera.transform.rotation = transform.rotation;
        transform.rotation = Quaternion.Euler(20, 0, 0);

        _followPlayer = PlayersVar.instance.GetGameObject(_owner)?.transform;
    }

    void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient) return;

        if(_followPlayer == null)
        {
            _followPlayer = PlayersVar.instance.GetGameObject(_owner)?.transform;
            return;
        }

        transform.position = _followPlayer.position;
    }
}
