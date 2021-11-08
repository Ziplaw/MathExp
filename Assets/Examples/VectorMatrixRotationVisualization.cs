using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorMatrixRotationVisualization : MonoBehaviour
{

    //Resources:
    // https://en.wikipedia.org/wiki/Rotation_matrix
    Vector3 dir;

    // Matrix[] rotationMatrices = new Matrix3x3[3];
    
    // private void Start() {
    //     rotationMatrices[0] = new Matrix4x4( // rotation in X
    //         new Vector4(,,,),
    //         new Vector4(,,,),
    //         new Vector4(,,,),
    //         new Vector4(,,,),


    //     ) 
    // }

    private void Start() {
        Debug.Log(transform.localToWorldMatrix);
        Debug.Log(transform.worldToLocalMatrix);
    }

    private void Update() {
        
        
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position,transform.position + dir);
    }
}
