using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Raytracing : MonoBehaviour
{
    public ComputeShader raytracingShader;
    RenderTexture target;
    
    public Texture SkyboxTexture;
    Camera _camera;
    int kernel;
    

    private void Awake() {
        _camera = GetComponent<Camera>();
        kernel = raytracingShader.FindKernel("CSMain");
    }

    void SetShaderParameters(){
        raytracingShader.SetMatrix("_CameraToWorld",_camera.cameraToWorldMatrix);
        raytracingShader.SetMatrix("_CameraInverseProjection",_camera.projectionMatrix.inverse);
        raytracingShader.SetTexture(kernel,"_SkyboxTexture",SkyboxTexture);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        SetShaderParameters();
        Render(dest);
    }

    private void Render(RenderTexture destination)
    {
        // Make sure we have a current render target
        InitRenderTexture();
        // Set the target and dispatch the compute shader
        raytracingShader.SetTexture(0, "Result", target);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        raytracingShader.Dispatch(kernel, threadGroupsX, threadGroupsY, 1);
        // Blit the result texture to the screen
        Graphics.Blit(target,destination);
    }

    private void InitRenderTexture()
    {
        if (target == null || target.width != Screen.width || target.height != Screen.height)
        {
            // Release render texture if we already have one
            if (target != null)
                target.Release();
            // Get a render target for Ray Tracing
            target = new RenderTexture(Screen.width, Screen.height, 0,
                RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            target.enableRandomWrite = true;
            target.Create();
        }

    }
}
