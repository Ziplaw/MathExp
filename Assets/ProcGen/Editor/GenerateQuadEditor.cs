using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenerateQuad))]
public class GenerateQuadEditor : Editor
{
    private GenerateQuad manager;

    private void OnEnable()
    {
        manager = target as GenerateQuad;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate"))
        {
            manager.Generate();
        }
        if (GUILayout.Button("Clear"))
        {
            manager.Clear();
        }
    }
}
