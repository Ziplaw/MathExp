using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalisationTester : MonoBehaviour
{
    public LocalisedString nameOfItem;

    private void Start()
    {
        Debug.Log((string)nameOfItem);
        Debug.Log("bruh");
        string text = nameOfItem;
    }
}
