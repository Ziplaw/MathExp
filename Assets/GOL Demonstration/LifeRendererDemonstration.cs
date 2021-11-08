using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LifeRendererDemonstration : MonoBehaviour
{
    public ComputeShader shader;
    public int resolution = 256;
    public Texture2D input;
    private RenderTexture renderTexture;
    private void Start()
    {
        renderTexture = new RenderTexture(resolution, resolution, 1);
        renderTexture.enableRandomWrite = true;
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.Create();

        if (input)
        {
            Graphics.Blit(input, renderTexture);
        }
        else
        {
            RenderTexture tex = new RenderTexture(resolution, resolution,1);
            tex.enableRandomWrite = true;
            tex.filterMode = FilterMode.Point;
            tex.Create();
            
            shader.SetInt("width",resolution);
            shader.SetTexture(1,"Generated",tex);
            shader.Dispatch(1,resolution/8,resolution/8,1);
            Graphics.Blit(tex, renderTexture);
        }

        shader.SetTexture(0,"Result",renderTexture);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(renderTexture, dest);
        
        shader.Dispatch(0, resolution/8, resolution/8, 1);
    }
}
