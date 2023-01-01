using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class KillPlane : MonoBehaviour
{
    void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {      
        CharacterFA collisionCharacter = other.gameObject.GetComponent<CharacterFA>();

        if (collisionCharacter && PhotonNetwork.IsMasterClient)
        {
            collisionCharacter.transform.position = collisionCharacter.lastWayPoint.position;
        }
    }
}
