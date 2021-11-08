using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolarCoordinateVisualizer : MonoBehaviour
{
    public float radius;
    public float offset;
    public int sides;
    public int timesRepeated = 4;


    private void OnDrawGizmos()
    {
        float offsetTemp = offset;
        for (int j = 0; j < timesRepeated; j++)
        {
            offset = offsetTemp + (2 * Mathf.PI * j / (float) timesRepeated);

            float x = radius * Mathf.Cos(2 * Mathf.PI + offset);
            float y = radius * Mathf.Sin(2 * Mathf.PI + offset);

            Vector3 prevPos = transform.position + new Vector3(x, y, 0);

            for (int i = 0; i <= sides; i++)
            {
                float t = i / (float) sides;

                x = radius * Mathf.Cos(2 * Mathf.PI * t + offset);
                y = radius * Mathf.Sin(2 * Mathf.PI * t + offset);

                Vector3 position = new Vector3(x, y, 0);

                Gizmos.DrawLine(prevPos, transform.position + position);
                prevPos = position;
            }
        }

        offset = offsetTemp;
    }
}