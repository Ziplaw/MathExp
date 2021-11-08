using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BezierPoint
{
    public Vector3 position;
    [HideInInspector] public Vector3 worldSpaceExitControl => position + localSpaceExitControl;
    [HideInInspector] public Vector3 worldSpaceEntryControl => position - localSpaceExitControl;
     public Vector3 localSpaceExitControl;
     // [HideInInspector] public Vector3 localSpaceEntryControl => -localSpaceExitControl;
     // [Range(0,1)]public float tilt;
     public Vector3 up = Vector3.up;
     public Vector3 displacementVector => Vector3.Cross(up, localSpaceExitControl.normalized);
}