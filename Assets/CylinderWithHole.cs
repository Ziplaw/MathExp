using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class CylinderWithHole : MonoBehaviour
{
    [Range(3,100)]public int resolution = 8;
    public float radiusInner = 8;
    public float radiusOuter = 12;
    public Material material;
    private int VertexCount => resolution * 2;
    private int TriCount => VertexCount * 3;

    private Vector3[] vertices;

    private void OnValidate()
    {
        Generate();
        // Debug.Log(VertexCount + " " + TriCount);
    }

    void Generate()
    {
        Mesh mesh = new Mesh();
        vertices = new Vector3[VertexCount];

        int[] triangles = new int[TriCount];

        //something something
        for (int i = 0; i < VertexCount; i++)
        {
            float t = i / (float) resolution;
            float TWO_PI = 2 * Mathf.PI;
            bool isInner = i < resolution;

            float x = isInner ? radiusInner * Mathf.Cos(TWO_PI * t) : radiusOuter * Mathf.Cos(TWO_PI * t);
            float y = isInner ? radiusInner * Mathf.Sin(TWO_PI * t) : radiusOuter * Mathf.Sin(TWO_PI * t);

            vertices[i] = new Vector3(x, y, 0);
        }

        int currentVertex = 0;

        for (int i = 0; i < resolution; i++)
        {
            if (i != resolution - 1)
            {
                triangles[currentVertex] = i;
                triangles[currentVertex + 1] = i + 1;
                triangles[currentVertex + 2] = i + resolution;
                triangles[currentVertex + 3] = i + resolution;
                triangles[currentVertex + 4] = i + 1;
                triangles[currentVertex + 5] = i + resolution + 1;
                currentVertex += 6;
            }
            else
            {
                triangles[currentVertex] = i;
                triangles[currentVertex + 1] = 0;
                triangles[currentVertex + 2] = i + resolution;
                triangles[currentVertex + 3] = i + resolution;
                triangles[currentVertex + 4] = 0;
                triangles[currentVertex + 5] = resolution;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        material = material ? material : new Material(Shader.Find("Standard"));
        GetComponent<MeshRenderer>().sharedMaterial = material;

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    private void OnDrawGizmos()
    {
        foreach (var vertex in vertices)
        {
            Gizmos.DrawWireSphere(vertex, .1f);
        }
    }
}