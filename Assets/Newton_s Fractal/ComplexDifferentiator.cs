using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class ComplexDifferentiator : MonoBehaviour
{
    public Complex iZ0;

    public Vector2 root;
    public Vector2 z0;
    public List<Vector2> path = new List<Vector2>();
    public int iterations;

    private void Start()
    {
        iZ0 = new Complex(.9f, 1);

        var iZ1 = fPrime(iZ0);
        
        // Debug.Log(f(iZ0));
        // Debug.Log(.Real + " " + fPrime(iZ0).Imaginary);
        
        
    }

    public Complex f(Complex z)
    {
        return (z - new Complex(1, 1)) * (z - new Complex(-3, -5));
    }

    public double epsilon = Double.Epsilon;
    public double gamma = Double.MaxValue;

    public Complex fPrime(Complex z)
    {
        return (f(z + epsilon) - f(z)) /epsilon;
    }

    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.green;
        // Gizmos.DrawCube(new Vector3((float)iRoot.Real, (float)iRoot.Imaginary, 0), Vector3.one * .1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawCube(new Vector3((float)iZ0.Real, (float)iZ0.Imaginary, 0), Vector3.one * .1f);

        // foreach (var vector2 in path)
        // {
        //     Gizmos.color = Color.cyan;
        //     Gizmos.DrawCube(vector2, Vector3.one * .1f);
        // }
    }
}