using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class GenerateQuad : MonoBehaviour
{
    [Range(1, 8)] public int resolution;
    [Range(1, 8)] public int renderDistance;
    [Range(0, 10)] public int detailDistance;
    private int exponentiatedResolution => (int) Mathf.Pow(2, resolution);
    public Vector2 size = new Vector2(1, 1);
    [Range(1.5f, 250)] public float height = 1;
    public Shader shader;
    public bool autoRefresh;
    public ComputeShader terrainGenShader;
    public Transform parent;

    [Space] [Header("Perlin Noise")] [Range(1, 10)]
    public int octaves = 100;

    public float amplitude = 1; //maximum amplitude
    public float frequency = 20; // minimum Size / Zoom
    [Range(0, 1)] public float persistence; // how quickly amplitude diminishes per octave
    [Range(1.1f, 10)] public float lacunarity;
    public float heightCoefficient = 1;
    public AnimationCurve heightCurve;
    public int heightSamples = 1000;

    float[] _heightCurveArray
    {
        get
        {
            float[] result = new float[heightSamples];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = heightCurve.Evaluate(i / (float) result.Length);
            }

            return result;
        }
    }

    private int step;
    private void Update()
    {
        if (autoRefresh && step % 1== 0)
            Generate();
        step++;
    }

    public void Clear()
    {
        while(parent.childCount != 0){
            DestroyImmediate(parent.GetChild(0).gameObject);
        }
    }
    public void Generate()
    {
        Clear();

        int tempRes = resolution;   
        for (int y = 0; y < renderDistance * 2 - 1; y++)
        {
            for (int x = 0; x < renderDistance * 2 - 1; x++)
            {


                GameObject chunk = new GameObject();
                chunk.transform.SetParent(parent);
                var parentPosition = parent.transform.position;
                
                chunk.transform.localPosition = new Vector3((x-renderDistance+1)*size.x,0,(y-renderDistance+1)*size.y);




                MeshFilter filter = chunk.AddComponent<MeshFilter>();
                MeshRenderer renderer = chunk.AddComponent<MeshRenderer>();

                Mesh mesh = new Mesh();

                Vector3[] vertices = new Vector3[exponentiatedResolution * exponentiatedResolution];

                ComputeBuffer vertexBuffer =
                    new ComputeBuffer(exponentiatedResolution * exponentiatedResolution, 3 * sizeof(float));
                ComputeBuffer heightCurveBuffer = new ComputeBuffer(_heightCurveArray.Length, sizeof(float));

                vertexBuffer.SetData(vertices);
                heightCurveBuffer.SetData(_heightCurveArray);


                var kernel = terrainGenShader.FindKernel("ComputeMesh");
                terrainGenShader.SetBuffer(kernel, "vertices", vertexBuffer);
                terrainGenShader.SetBuffer(kernel, "heightCurve", heightCurveBuffer);
                terrainGenShader.SetVector("size", size);

                int distance = Mathf.Abs(x- renderDistance+1)+Mathf.Abs(y- renderDistance+1)-detailDistance;
                distance = Mathf.Max(0, distance);

                resolution -= distance;
                resolution = Mathf.Max(resolution, 1);
                chunk.name = "Chunk " + resolution;
                
                int distanceCoefficient = 0; // PROBABLEMENTE HAYA QUE HACER DOS FORS PARA X E Y // Confirmado
                var position = chunk.transform.position;
                terrainGenShader.SetVector("offset", new Vector4(position.x, position.z, 0, 0));
                terrainGenShader.SetInt("resolution", exponentiatedResolution);
                terrainGenShader.SetFloat("height", height);
                terrainGenShader.SetFloat("octaves", octaves);
                terrainGenShader.SetFloat("amplitude", amplitude);
                terrainGenShader.SetFloat("frequency", frequency);
                terrainGenShader.SetFloat("persistence", persistence);
                terrainGenShader.SetFloat("lacunarity", lacunarity);
                terrainGenShader.SetFloat("heightCoefficient", heightCoefficient);
                terrainGenShader.SetInt("heightSamples", heightSamples);

                terrainGenShader.Dispatch(kernel, exponentiatedResolution, exponentiatedResolution, 1);

                vertexBuffer.GetData(vertices);
                vertexBuffer.Dispose();
                heightCurveBuffer.Dispose();

                int triAmount = 6 * (exponentiatedResolution - 1) * (exponentiatedResolution - 1);
                int[] triangles = new int[triAmount];
                int arrayIndex = 0;
                kernel = terrainGenShader.FindKernel("ComputeTriangles");
                ComputeBuffer triangleBuffer = new ComputeBuffer(triAmount, sizeof(int));
                terrainGenShader.SetBuffer(kernel, "triangles", triangleBuffer);
                terrainGenShader.Dispatch(kernel, (exponentiatedResolution * exponentiatedResolution) - 1, 1, 1);
                triangleBuffer.GetData(triangles);
                triangleBuffer.Dispose();

                // Debug.Log(triangles);
                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.RecalculateNormals();
                mesh.name = "Procedural Quad";

                renderer.sharedMaterial = new Material(shader);
                renderer.sharedMaterial.color = new Color(0.45f, 0.32f, 0.09f);
                filter.sharedMesh = mesh;
                
                resolution = tempRes;
            }
        }

    }
}