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
        localPlayer = PhotonNetwork.LocalPlayer;
        localPlayer.NickName = nickName;
    }


    void Update()
    {
        if (!MyServer.Instance.canPlay) return;
        
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

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            MyServer.Instance.RequestTurboOn(localPlayer);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            MyServer.Instance.RequestTurboOff(localPlayer);
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
