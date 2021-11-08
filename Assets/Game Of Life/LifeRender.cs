using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LifeRender : MonoBehaviour
{
    public ComputeShader shader;
    public int width = 256;
    public int height = 256;

    struct Cell
    {
        public Vector2 pos;
        public int alive;
    }

    Cell[] cells;
    private RenderTexture renderTexture;
    private GameObject[] cubes;

    private void Start()
    {
        renderTexture = new RenderTexture(width, height, 1, RenderTextureFormat.Default);
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        cells = new Cell[width * height];
        cubes = new GameObject[width * height];

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].pos = new Vector2(i % width, i / height);
            cells[i].alive = Random.value > .5f ? 1 : 0;
            cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            Destroy(cubes[i].GetComponent<Collider>());
            cubes[i].transform.position = cells[i].pos;
            cubes[i].transform.SetParent(transform);
        }
        shader.SetInt("height", height);
        shader.SetInt("width", width);
        shader.SetTexture(0, "Result", renderTexture);

    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        int size = 2 * sizeof(float) + sizeof(int);
        ComputeBuffer cellBuffer = new ComputeBuffer((int) (width * height), size);
        cellBuffer.SetData(cells);

        
        shader.SetBuffer(0, "cells", cellBuffer);
        shader.Dispatch(0, width / 8, height / 8, 1);
        cellBuffer.GetData(cells);
        cellBuffer.Dispose();

        for (int i = 0; i < cells.Length; i++)
        {
            cubes[i].SetActive(cells[i].alive == 1);
        }
        
        
    }
}