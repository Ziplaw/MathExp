using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour
{
    public Path path;
    [Range(0, 1)] public float position;

    private void OnValidate()
    {
        transform.position = path.GetPosition(position);
        transform.rotation = path.GetRotation(position);
    }
}
