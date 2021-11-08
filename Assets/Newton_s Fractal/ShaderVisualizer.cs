using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteAlways]
public class ShaderVisualizer : MonoBehaviour
{
    public ComputeShader shader;
    private RenderTexture _texture;
    [Range(0, 10)] public int iterations;
    private int kernel;
    public Vector2 offset;
    public List<Point> points;

    [Serializable]
    public struct Point
    {
        public Vector2 pos;
        public Color col;
    }

    public int resolution = 512;

    // Start is called before the first frame update
    void Awake()
    {
        kernel = shader.FindKernel("CSMain");
        _texture = new RenderTexture(resolution, resolution, 1, RenderTextureFormat.ARGB32)
        {
            enableRandomWrite = true
        };
    }

    int _selectedPointIndex = int.MaxValue;
    public float dimension = 4;
    public bool render;

    [ExecuteAlways]
    private void Update()
    {
        var fixedMouseCoords = (Input.mousePosition - new Vector3(resolution * .5f, resolution * .5f, 0)) * dimension / resolution;

        if (Input.GetMouseButtonDown(0))
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                if (Vector2.Distance(point.pos, fixedMouseCoords) < .05)
                {
                    _selectedPointIndex = i;
                    break;
                }
            }

        if (Input.GetMouseButtonUp(0))
        {
            _selectedPointIndex = int.MaxValue;
        }

        if (_selectedPointIndex != int.MaxValue)
        {
            var point = points[_selectedPointIndex];
            point.pos =  Vector2.Lerp( point.pos, fixedMouseCoords, .02f);
            points[_selectedPointIndex] = point;
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (render)
        {
            ComputeBuffer pointBuffer = new ComputeBuffer(points.Count, 24);
            pointBuffer.SetData(points);
            shader.SetBuffer(kernel, "points", pointBuffer);
            shader.SetTexture(kernel, "Result", _texture);
            shader.SetFloat("resolution", resolution);
            shader.SetInt("pointAmount", points.Count);
            shader.SetInt("iterations", iterations);
            shader.SetFloat("dimensions", dimension);
            shader.SetVector("offset", offset);
            shader.Dispatch(kernel, resolution / 8, resolution / 8, 1);
            pointBuffer.Dispose();
            Graphics.Blit(_texture, dest);
        }
        else
        {
            Graphics.Blit(src,dest);
        }
        
    }
}