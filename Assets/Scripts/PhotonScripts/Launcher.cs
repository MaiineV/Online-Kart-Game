using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    public MyServer serverPrefab;
    public ControllerFA controllerPrefab;
    public PlayersVar playerVarPrefab;

    [SerializeField] string _actualNick;
    [SerializeField] InputField inputField;

    public void BTN_Connect()
    {
        ReviceInputField();
        PhotonNetwork.ConnectUsingSettings();
    }

    public void ReviceInputField()
    {
        Debug.Log(inputField.text);
        _actualNick = inputField.text;
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;

        PhotonNetwork.JoinOrCreateRoom("ServerFullAuth", options, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        PhotonNetwork.Instantiate(serverPrefab.name, Vector3.zero, Quaternion.identity);
        PhotonNetwork.Instantiate(playerVarPrefab.name, Vector3.zero, Quaternion.identity);
    }

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Instantiate(controllerPrefab).SetInitial(_actualNick);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Connection failed: " + cause.ToString());
    }
}
