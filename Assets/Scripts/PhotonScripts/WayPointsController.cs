using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointsController : MonoBehaviour
{
    [SerializeField] int _waypointNumber;

    void Start()
    {
        PlayersVar.instance.AddWayPoint(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterFA collisionCharacter = other.gameObject.GetComponent<CharacterFA>();

        if (collisionCharacter)
        {
            PlayersVar.instance.RequestChangeWayPoint(collisionCharacter.owner, _waypointNumber);
        }
    }
}
