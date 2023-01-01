using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CameraPhoton : MonoBehaviour
{
    Camera _mainCamera;
    Player _owner;
    Transform _rotationParent;
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
        _rotationParent = PlayersVar.instance.GetGameObject(_owner)?.GetComponent<CharacterFA>().rotationPoint;
    }

    void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient) return;

        if(_followPlayer == null)
        {
            _followPlayer = PlayersVar.instance.GetGameObject(_owner)?.transform;
            return;
        }

        if (_rotationParent == null)
        {
            _rotationParent = PlayersVar.instance.GetGameObject(_owner)?.GetComponent<CharacterFA>().rotationPoint;
            return;
        }

        transform.rotation = _rotationParent.rotation;
        transform.position = _followPlayer.position;
    }
}
