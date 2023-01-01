using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System;
using System.IO;

public class PlayersVar : MonoBehaviour
{
    public static PlayersVar instance;

    [SerializeField] PhotonView _photonView;
    public CanvasController _canvas;

    Dictionary<Player, GameObject> _playersGameObjects = new Dictionary<Player, GameObject>();
    List<Player> players = new List<Player>();

    List<WayPointsController> raceWayPoints = new List<WayPointsController>();

    public string path = "Assets/Serializacion/data/";
    public string fileName = "TheInfo";
    public string username, password;

    public ListSerialize myInfo;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
        _canvas = FindObjectOfType<CanvasController>();
    }

    public void ReciveCanvas(CanvasController canvas)
    {
        _canvas = canvas;
    }

    public void AddPlayerGameObject(Player actualPlayer, GameObject actualGameObject)
    {
        if (!_playersGameObjects.ContainsKey(actualPlayer))
        {
            _playersGameObjects.Add(actualPlayer, actualGameObject);
        }
    }

    public GameObject GetGameObject(Player actualPlayer)
    {
        if (_playersGameObjects.ContainsKey(actualPlayer))
            return _playersGameObjects[actualPlayer];
        else
            return null;
    }

    public void AddPlayer(Player actualPlayer)
    {
        if (!players.Contains(actualPlayer) && PhotonNetwork.IsMasterClient)
        {       
            _photonView.RPC("AddPlayerToAll", MyServer.Instance._server, actualPlayer);
        }
    }

    [PunRPC]
    void AddPlayerToAll(Player actualPlayer)
    {
        Debug.Log("me anadi");
        players.Add(actualPlayer);

        List<Player> tempList = players;
        tempList.OrderByDescending(x => x.laps).ThenByDescending(x => x.waypoint);

        int index = 1;

        foreach (Player item in tempList)
        {
            if (item == actualPlayer)
                break;

            index++;
        }

        _photonView.RPC("ChangePlayerText", actualPlayer, actualPlayer.laps);
        _photonView.RPC("UpdatePosition", actualPlayer, index, tempList.Count);

    }

    public void AddWayPoint(WayPointsController waypoint)
    {
        if (!raceWayPoints.Contains(waypoint))
            raceWayPoints.Add(waypoint);
    }

    public void RequestChangeWayPoint(Player actualPlayer, int wayPoint)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        ChangeWayPoint(actualPlayer, wayPoint);
        //_photonView.RPC("ChangeWayPoint", RpcTarget.All, actualPlayer, wayPoint);
    }

    void ChangeWayPoint(Player actualPlayer, int wayPoint)
    {
        if (!players.Contains(actualPlayer)) return;

        if (wayPoint != 0 && actualPlayer.waypoint == wayPoint - 1)
        {
            actualPlayer.waypoint = wayPoint;
        }
        else if (wayPoint == 0 && actualPlayer.waypoint == raceWayPoints.Count - 1)
        {
            actualPlayer.waypoint = wayPoint;
            actualPlayer.laps++;

            if (actualPlayer.laps > 3)
            {
                _photonView.RPC("TriggerWinScreen", actualPlayer);
            }
        }


        List<Player> tempList = players;
        tempList.OrderByDescending(x => x.waypoint).ThenByDescending(x => x.laps);

        int index = 1;

        foreach (Player item in tempList)
        {
            _photonView.RPC("UpdatePosition", item, index, tempList.Count);
            _photonView.RPC("ChangePlayerText", item, item.laps);

            Debug.Log(item.NickName + " " + index);
            Debug.Log(item.NickName + " " + item.laps);
            Debug.Log(item.NickName + " " + item.waypoint);

            index++;
        }
    }

    [PunRPC]
    void ChangePlayerText(int laps)
    {
        if (!_canvas)
        {
            _canvas = FindObjectOfType<CanvasController>();
            Debug.Log("lo busque");
        }

        _canvas.ChangeLapCounter(laps);
    }

    [PunRPC]
    void UpdatePosition(int actualPos, int maxPlayers)
    {
        if (!_canvas)
        {
            _canvas = FindObjectOfType<CanvasController>();
            Debug.Log("lo busque");
        }

        _canvas?.ChangePositionCounter(actualPos, maxPlayers);
    }

    [PunRPC]
    void TriggerWinScreen()
    {
        _canvas.WinScreen();
    }

    private void SerializeJSON()
    {
        //Creamos el archivo en su respectivo directorio
        StreamWriter file = File.CreateText(path + fileName + ".json");

        //Pasamos la info a string (json)
        string json = JsonUtility.ToJson(myInfo, true);

        //Guardamos la info en el archivo
        file.Write(json);

        //Cerramos el archivo
        file.Close();
    }

    void DeserializeJSON()
    {
        string finalPath = path + fileName + ".json";

        //Existe el archivo?
        if (!File.Exists(finalPath)) return;

        string json = File.ReadAllText(finalPath);

        myInfo = JsonUtility.FromJson<ListSerialize>(json);
    }
}
