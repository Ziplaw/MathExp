using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMaker : MonoBehaviour
{
    public int pointAmount;

    private void OnDrawGizmos()
    {
        for (int i = 0; i <= pointAmount; i++)
        {
            float t = (i / (float)pointAmount) * 2 * Mathf.PI;
            
            float y = Mathf.Sin(t);
            float x = Mathf.Cos(t);
        }
    }
}
