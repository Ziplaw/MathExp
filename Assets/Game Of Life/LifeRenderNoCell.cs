using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Random = UnityEngine.Random;

public class LifeRenderNoCell : MonoBehaviour
{
    public ComputeShader shader;
    public Texture2D input;
    public int width => input.width;
    public int height => input.height;

    private RenderTexture renderTexture;

    public int resolution = 8192;
    

    private void Start()
    {
        renderTexture = new RenderTexture(input? width : resolution, input ? height : resolution, 1);
        renderTexture.enableRandomWrite = true;
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.Create();
        
        if (input)
        {
            Graphics.Blit(input, renderTexture);
        }
        else
        {
            Texture2D texture = new Texture2D(resolution, resolution, TextureFormat.RGB24,false,false);
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    texture.SetPixel(x,y,Random.value > .5f ? Color.white : Color.black);
                }
            }
            
            texture.Apply();
            
            Graphics.Blit(texture,renderTexture);
        }
        shader.SetTexture(0, "Result", renderTexture);
        
        Debug.Log(renderTexture.width);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // shader.SetTexture(0, "Result", renderTexture);
        Graphics.Blit(renderTexture, dest);
        shader.Dispatch(0, input ? width : resolution / 8, input ?height : resolution / 8, 1);

    }
}