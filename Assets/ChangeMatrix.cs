using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMatrix : MonoBehaviour
{
    public Matrix4x4 cameraMatrix;
    private Matrix4x4 matrixCopy;
    public GameObject clip;
    
    
    void Start()
    {
        matrixCopy = GetComponent<Camera>().cullingMatrix;
    }

    // Update is called once per frame
    void Update()
    {
        Plane p = new Plane(clip.transform.up, clip.transform.position);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(GetComponent<Camera>().worldToCameraMatrix)) * clipPlane;
        GetComponent<Camera>().projectionMatrix = GetComponent<Camera>().CalculateObliqueMatrix(clipPlaneCameraSpace);
    }
}
