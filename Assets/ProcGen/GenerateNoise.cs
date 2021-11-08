using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GenerateNoise : MonoBehaviour
{
    public ComputeShader shader;
    public RenderTexture noiseTexture;
    [Range(1,100)]public int octaves = 8;
    public float amplitude = 1; //maximum amplitude
    public float frequency = 20; // minimum Size / Zoom
    [Range(0,1)]public float persistence; // how quickly amplitude diminishes per octave
    [Range(1.1f,100)]public float lacunarity; // how quickly frequency increases per octave

    private int kernelIndex;

    private void Start()
    {
        noiseTexture = new RenderTexture(256,256,1);
        noiseTexture.enableRandomWrite = true;
        noiseTexture.Create();
        
        kernelIndex = shader.FindKernel("GenerateNoise");
        shader.SetTexture(kernelIndex,"noise", noiseTexture);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        shader.SetFloat("octaves",octaves);
        shader.SetFloat("amplitude",amplitude);
        shader.SetFloat("frequency",frequency);
        shader.SetFloat("persistence",persistence);
        shader.SetFloat("lacunarity",lacunarity);
        
        shader.Dispatch(kernelIndex, 32,32,1);
        Graphics.Blit(noiseTexture, dest);
    }
}
