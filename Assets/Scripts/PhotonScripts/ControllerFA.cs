using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ControllerFA : MonoBehaviourPun
{
    public Player localPlayer;
    string nickName;

    float _h;
    float _v;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Debug.Log("1");
        localPlayer = PhotonNetwork.LocalPlayer;
        localPlayer.NickName = nickName;
        Debug.Log(localPlayer.NickName);
    }


    void Update()
    {
        _h = Input.GetAxis("Horizontal");
        _v = Input.GetAxis("Vertical");

        if (_h != 0 || _v != 0)
        {
            MyServer.Instance.RequestMove(localPlayer, _h, _v);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            MyServer.Instance.RequestJump(localPlayer);
        }
    }

    public void SetInitial(string nick)
    {
        Debug.Log(nick);
        nickName = nick;
        Debug.Log("2");


    }

    public Player GetPlayer()
    {
        return localPlayer;
    }
}
