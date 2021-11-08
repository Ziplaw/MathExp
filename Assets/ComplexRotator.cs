using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexRotator : MonoBehaviour
{
    public float rotationInDegrees;
    public Vector2 pointToRotate;
    private Vector2 _rotatedPoint;

    private void OnValidate()
    {
        //We take the rotation on a unit circle and we multiply it by the vector in complex space
        //so the rotation would be cos(x) + sin(x) * i
        //and the complex vector would be point.x + point.y * i
        //where i^2 = -1
        
        // https://media.discordapp.net/attachments/698525973552955483/828999135419301918/unknown.png?width=953&height=538

        var cosx = Mathf.Cos(rotationInDegrees * Mathf.Deg2Rad);
        var sinx = Mathf.Sin(rotationInDegrees * Mathf.Deg2Rad);

        var x = pointToRotate.x;
        var y = pointToRotate.y;

        var rotatedX = x * cosx - y * sinx;
        var rotatedY = x * sinx + y * cosx;
        
        _rotatedPoint = new Vector2(rotatedX, rotatedY);

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.TransformPoint(pointToRotate),.1f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.TransformPoint(_rotatedPoint),.1f);
    }
}
