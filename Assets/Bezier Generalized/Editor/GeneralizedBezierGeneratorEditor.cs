using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GeneralizedBezierGenerator))]
public class GeneralizedBezierGeneratorEditor : Editor
{
    GeneralizedBezierGenerator manager;
    
    void OnEnable()
    {
        manager = target as GeneralizedBezierGenerator;
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
