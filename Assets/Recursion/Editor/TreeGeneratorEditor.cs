using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TreeGenerator))]
public class TreeGeneratorEditor : Editor
{
    private TreeGenerator manager;

    private void OnEnable()
    {
        manager = target as TreeGenerator;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate"))
        {
            manager.Generate();
        }
        if (GUILayout.Button("Delete"))
        {
            manager.DeleteBranches();
        }
    }
}
