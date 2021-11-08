using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCurveManipulator : MonoBehaviour
{
    public AnimationCurve terrainCurve;
    public int sampleNumber = 100;

    float[] samples
    {
        get
        {
            float[] result = new float[sampleNumber];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = terrainCurve.Evaluate(i / (float) result.Length);
            }

            return result;
        }
    }

    private void OnValidate()
    {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        Renderer renderer = GetComponent<Renderer>();
        
        renderer.GetPropertyBlock(propertyBlock);
        // Assign our new value.
        propertyBlock.SetFloatArray("height",samples);
        propertyBlock.SetInt("heightNumberOfSamples",sampleNumber);
        // Apply the edited values to the renderer.
        renderer.SetPropertyBlock(propertyBlock);
    }
}
