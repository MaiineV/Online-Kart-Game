using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayersVar : MonoBehaviour
{
    public static PlayersVar instance;

    [SerializeField] PhotonView _photonView;

    Dictionary<Player, GameObject> _playersGameObjects = new Dictionary<Player, GameObject>();
    Dictionary<Player, int> _playersLaps = new Dictionary<Player, int>();
    Dictionary<Player, int> _playersWayPoints = new Dictionary<Player, int>();

    List<WayPointsController> raceWayPoints = new List<WayPointsController>();

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
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

    public void AddPlayerToLaps(Player actualPlayer)
    {
        if (!_playersLaps.ContainsKey(actualPlayer))
        {
            _playersLaps.Add(actualPlayer, 0);
        }

        if (!_playersWayPoints.ContainsKey(actualPlayer))
        {
            _playersWayPoints.Add(actualPlayer, 0);
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

        _photonView.RPC("ChangeWayPoint", RpcTarget.All, actualPlayer, wayPoint);
    }

    [PunRPC]
    void ChangeWayPoint(Player actualPlayer, int wayPoint)
    {
        Debug.Log("pre if");
        if (!_playersWayPoints.ContainsKey(actualPlayer) || !_playersLaps.ContainsKey(actualPlayer)) return;

        if (wayPoint != 0 && _playersWayPoints[actualPlayer] == wayPoint - 1)
        {
            _playersWayPoints[actualPlayer] = wayPoint;
        }
        else if (wayPoint == 0 && _playersWayPoints[actualPlayer] == raceWayPoints.Count - 1)
        {
            _playersWayPoints[actualPlayer] = wayPoint;
            _playersLaps[actualPlayer]++;

            if (_playersLaps[actualPlayer] >= 3)
            {
                //Victory screen
            }
        }

        Debug.Log(_playersLaps[actualPlayer]);
        Debug.Log(_playersWayPoints[actualPlayer]);
    }
}
