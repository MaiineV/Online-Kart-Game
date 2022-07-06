using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System;

public class PlayersVar : MonoBehaviour
{
    public static PlayersVar instance;

    [SerializeField] PhotonView _photonView;
    public CanvasController _canvas;

    Dictionary<Player, GameObject> _playersGameObjects = new Dictionary<Player, GameObject>();
    List<Player> players = new List<Player>();

    List<WayPointsController> raceWayPoints = new List<WayPointsController>();

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
        if (!players.Contains(actualPlayer))
        {
            players.Add(actualPlayer);
            _photonView.RPC("UpdatePositionScore", actualPlayer);
        }
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
    }

    void ChangeWayPoint(Player actualPlayer, int wayPoint)
    {
        if (!players.Contains(actualPlayer)) return;

        if (wayPoint != 0 && actualPlayer.waypoint == wayPoint - 1)
        {
            actualPlayer.waypoint = wayPoint;
            _photonView.RPC("UpdatePositionScore", actualPlayer, actualPlayer.waypoint, actualPlayer.laps, actualPlayer.NickName);
        }
        else if (wayPoint == 0 && actualPlayer.waypoint == raceWayPoints.Count - 1)
        {
            actualPlayer.waypoint = wayPoint;
            actualPlayer.laps++;
            _photonView.RPC("ChangePlayerText", actualPlayer, actualPlayer.laps);
            _photonView.RPC("UpdatePositionScore", actualPlayer);

            if (actualPlayer.laps > 3)
            {
                _photonView.RPC("TriggerWinScreen", actualPlayer);
            }
        }
    }

    [PunRPC]
    void ChangePlayerText(int laps)
    {
        _canvas.ChangeLapCounter(laps);
    }

    [PunRPC]
    void UpdatePositionScore()
    {
        List<Player> tempList = players;
        tempList.OrderByDescending(x => x.laps).ThenByDescending(x => x.waypoint);

        if (!_canvas)
        {
            _canvas = FindObjectOfType<CanvasController>();
            Debug.Log("lo busque");
        }

        _canvas?.ChangeScoreboard(tempList);
    }

    [PunRPC]
    void TriggerWinScreen()
    {
        _canvas.WinScreen();
    }
}
