using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public Transform bluePortal,orangePortal,bluePortalCamera,bluePortalQuad,orangePortalCamera,orangePortalQuad,playerTransform, playerCamera;
    public bool onGoingTP;
    public static PortalManager i;

    public Action<Transform> OnPlayerTeleport;

    private void Awake() {
        i = this;
        OnPlayerTeleport += TeleportPlayerTo;
    }



    private void Update() {
        orangePortalCamera.localPosition =  playerCamera.position - bluePortalQuad.position;
        orangePortalCamera.localRotation = playerCamera.localRotation;

        bluePortalCamera.localPosition =  playerCamera.position - orangePortalQuad.position;
        bluePortalCamera.localRotation = playerCamera.localRotation;
    }

    public void TeleportPlayerTo(Transform portalQuad){

        onGoingTP = true;
        Vector3 prevPortalPos = playerTransform.position - portalQuad.position;
        Vector3 newPos = portalQuad == orangePortalQuad? bluePortalQuad.position : orangePortalQuad.position;
        newPos += prevPortalPos;

        Quaternion newRotation = portalQuad == orangePortalQuad ? bluePortalCamera.rotation : orangePortalCamera.rotation;


        playerTransform.position = newPos;
        PortalManager.i.playerCamera.rotation = newRotation;
    }
}
