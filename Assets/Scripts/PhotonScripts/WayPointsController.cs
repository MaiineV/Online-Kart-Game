using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointsController : MonoBehaviour
{
    [SerializeField] int _waypointNumber;

    void Start()
    {
        PlayersVar.instance.AddWayPoint(this);
        GetComponent<Renderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterFA collisionCharacter = other.gameObject.GetComponent<CharacterFA>();

        if (collisionCharacter)
        {
            collisionCharacter.lastWayPoint = transform;
            PlayersVar.instance.RequestChangeWayPoint(collisionCharacter.owner, _waypointNumber);
        }
    }
}
