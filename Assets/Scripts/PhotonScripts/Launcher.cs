using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public MyServer serverPrefab;
    public ControllerFA controllerPrefab;

    public void BTN_Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 16;

        PhotonNetwork.JoinOrCreateRoom("ServerFullAuth", options, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        PhotonNetwork.Instantiate(serverPrefab.name, Vector3.zero, Quaternion.identity);
    }

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Instantiate(controllerPrefab);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Connection failed: " + cause.ToString());
    }
}
