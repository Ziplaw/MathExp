using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    bool inside;
    Transform playerInside;
    Vector3 newPos, prevPos;
    bool localOnGoingTP;
    float previousDotSign => Mathf.Sign(Vector3.Dot(prevPos - transform.position,transform.forward));
    float newDotSign => Mathf.Sign(Vector3.Dot(newPos - transform.position,transform.forward));
    private void Update() {
        if(inside)
        {
            newPos = playerInside.position;
            if(previousDotSign != newDotSign && !PortalManager.i.onGoingTP){
                 PortalManager.i.OnPlayerTeleport?.Invoke(transform);
                 localOnGoingTP = true;
            }
            prevPos = newPos;
        }
        else localOnGoingTP = false;
    }
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            inside = true;
            print("inside");
            playerInside = other.transform;
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player")) {
            inside = false;
            print("outside");
            if(!localOnGoingTP) PortalManager.i.onGoingTP = false;

            playerInside = null;
        }
    }
}
