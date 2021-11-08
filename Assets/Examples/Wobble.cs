using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Wobble : MonoBehaviour
{
    public Matrix4x4 originalProjection;
    Camera cam;

    void OnValidate()
    {
        cam = GetComponent<Camera>();
        originalProjection = cam.projectionMatrix;

        print(originalProjection);
    }
}
