using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralizedBezierGenerator : MonoBehaviour
{
    public Vector2[] points = new Vector2[2];
    public Vector3 position;
    [Range(0,1)]public float t;
    public int samples = 50;
    public Vector3[] pointsInCurve;
    
    private void OnValidate()
    {
        position = BezierPoint(points, t);
        
        pointsInCurve = new Vector3[samples];

        for (int i = 0; i < samples * t; i++)
        {
            pointsInCurve[i] = BezierPoint(points,  (i /(float) samples));
        }

    }

    private void Start()
    {
        print(Factorial(5));
    }

    Vector3 BezierPoint(Vector2[] points, float t)
    {
        Vector2 sum = Vector3.zero;
        int n = points.Length-1;
        
        for (int i = 0; i < points.Length; i++)
        {
            sum += BinomialCoefficient(n, i) * points[i] * Mathf.Pow((1 - t), n - i) * Mathf.Pow(t, i);
        }

        return sum;
    }

    int BinomialCoefficient(int n, int i)
    {
        var s = Factorial(n) / (Factorial(i) * Factorial(n - i));
        // Debug.Log(n + " " + i + " " + s);
        return s;
    }

    int Factorial(int n)
    {
        int temp = n;
        
        if (n == 0) return 1;
        
        for (int i = 2; i < temp; i++)
        {
            n *= i;
        }

        return n;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(position,1);
        Gizmos.color = Color.red;

        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawWireSphere(points[i],.1f);
        }


        Gizmos.color = Color.cyan;
        for (int i = 1; i < samples; i++)
        {
            if(pointsInCurve[i] != Vector3.zero)
            Gizmos.DrawLine(pointsInCurve[i-1],pointsInCurve[i]);
        }
        
        
    }
}
