using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ControllerFA : MonoBehaviourPun
{
    Player _localPlayer;

    float _h;
    float _v;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        _localPlayer = PhotonNetwork.LocalPlayer;
    }


    void Update()
    {
        _h = Input.GetAxis("Horizontal");
        _v = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            MyServer.Instance.RequestJump(_localPlayer);
        }
    }

    private void FixedUpdate()
    {
        if (_h != 0 || _v != 0)
        {
            MyServer.Instance.RequestMove(_localPlayer, _h, _v);
        }

        //MyServer.Instance.RequestGravity(_localPlayer);
    }

    public Player GetPlayer()
    {
        return _localPlayer;
    }
}
