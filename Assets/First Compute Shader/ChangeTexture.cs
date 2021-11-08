using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTexture : MonoBehaviour
{
    public ComputeShader compute;
    public RenderTexture texture;
    public int lSize = 8;

    private void Start() {
        texture = new RenderTexture(lSize,lSize,1);
        texture.enableRandomWrite = true;
        texture.filterMode = FilterMode.Point;
        texture.Create();

        // GetComponent<Renderer>().material.SetTexture("_MainTex",texture);

        Renderer renderer = GetComponent<Renderer>();
        MaterialPropertyBlock prop = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(prop);
        prop.SetTexture("_MainTex",texture);
        renderer.SetPropertyBlock(prop);
    }

    private void Update()
    {
        int kernel = compute.FindKernel("CSMain");
        compute.SetTexture(kernel,"Result",texture);
        compute.SetFloat("time",Time.time);
        compute.SetFloat("texRes",lSize * lSize);
        compute.Dispatch(kernel,lSize,lSize,1);


    }
}
