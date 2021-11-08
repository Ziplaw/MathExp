using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class RollableProcGen : MonoBehaviour
{
    [Range(0, 1)] public float roll;
    [Range(3, 100)] public int resolution = 8;
    public float radius = 1;
    public float radiusInner = 1;
    public float radiusOuter = 2;
    private int VertexCount => resolution * 2;
    private int TriCount => VertexCount * 3;

    public float ratio => resolution / (float)(resolution*2-2);


    private Vector3[] vertices;
    private Material material;

    private void OnValidate()
    {
        Generate();
    }

    private void Generate()
    {
        Mesh mesh = new Mesh();
        vertices = new Vector3[VertexCount];

        int[] triangles = new int[TriCount];

        for (int i = 0; i < VertexCount; i += 2)
        {
            float t = i / (float) resolution;
            float TWO_PI = 2 * Mathf.PI * roll*ratio;


            float xIn = radiusInner * Mathf.Cos(TWO_PI * t);
            float xOut = radiusOuter * Mathf.Cos(TWO_PI * t);
            float yIn = radiusInner * Mathf.Sin(TWO_PI * t);
            float yOut = radiusOuter * Mathf.Sin(TWO_PI * t);
            
            vertices[i] = new Vector3(xIn,yIn,0);
            vertices[i+1] = new Vector3(xOut,yOut,0);
        }

        int currentVertex = 0;
        
        for (int i = 0; i < resolution*2-2; i+=2)
        {
            // if (i != resolution - 1)
            {
                triangles[currentVertex] = i;
                triangles[currentVertex + 1] = i + 2;
                triangles[currentVertex + 2] = i + 1;
                triangles[currentVertex + 3] = i + 2;
                triangles[currentVertex + 4] = i + 3;
                triangles[currentVertex + 5] = i + 1;
                currentVertex += 6;
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
            Gizmos.DrawWireSphere(vertex, .01f);
        }
    }
}