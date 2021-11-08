using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    Vector3[] vertices = new Vector3[4]// exponentiatedResolution*exponentiatedResolution
    {
        new Vector3(0,0,0),
        new Vector3(0,1,0),
        new Vector3(1,0,0),
        new Vector3(1,1,0)
    };

    int[] tris = new int[6] // ???????
    {
        0,1,3,
        0,3,2
    };

    Vector3[] normals = new Vector3[4] //exponentiatedResolution * exponentiatedResolution
    {
        -Vector3.forward,
        -Vector3.forward,
        -Vector3.forward,
        -Vector3.forward
    };

    Vector2[] uv = new Vector2[4] //exponentiatedResolution * exponentiatedResolution
    {
      new Vector2(0, 0),
      new Vector2(1, 0),
      new Vector2(0, 1),
      new Vector2(1, 1)
    };

    private void Start() {
        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        filter.mesh = mesh;
    }
}
