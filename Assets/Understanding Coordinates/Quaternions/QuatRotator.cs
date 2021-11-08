using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuatRotator : MonoBehaviour
{
    public Vector3 axis = Vector3.up;
    public float degrees = Mathf.PI * Mathf.Rad2Deg;
    private void OnValidate()
    {
        Vector3 vectorPart = axis.normalized * Mathf.Sin(degrees * Mathf.Deg2Rad * .5f);
        float scalarPart = Mathf.Cos(degrees * Mathf.Deg2Rad * .5f);
        
        Quaternion rot = new Quaternion(vectorPart.x,vectorPart.y,vectorPart.z,scalarPart);

        transform.rotation = rot;
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.cyan;
        Handles.ArrowHandleCap(0,transform.position,Quaternion.LookRotation(axis.normalized),1,EventType.Repaint);
        
    }
}
