using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class VertexPainter : MonoBehaviour
{
    public ComputeShader shader;
    public GameObject plane;
    private Renderer _renderer;
    private RenderTexture renderTexture;
    private Camera camera;

    private void Start()
    {
        _renderer = plane.GetComponent<Renderer>();
        renderTexture = new RenderTexture(512,512,1,RenderTextureFormat.Default,3);
        renderTexture.enableRandomWrite = true;
        camera = GetComponent<Camera>();
    }

    private void Update()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        Physics.Raycast(ray, out hit,100000, 1<<LayerMask.NameToLayer("Water"));
        
        var index = shader.FindKernel("CSMain");
        
        shader.SetVector("position",hit.textureCoord * renderTexture.width);
        shader.SetTexture(index,"Result",renderTexture);
        shader.Dispatch(index,renderTexture.width / 8,renderTexture.width / 8,1);
        
        
        _renderer.sharedMaterial.mainTexture = renderTexture;
    }

    private void LateUpdate()
    {
        plane.GetComponent<MeshCollider>().sharedMesh = null;
        plane.GetComponent<MeshCollider>().sharedMesh = plane.GetComponent<MeshFilter>().sharedMesh;
    }
}
