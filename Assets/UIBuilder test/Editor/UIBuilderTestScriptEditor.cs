using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIBuilderTestScript))]
public class UIBuilderTestScriptEditor : Editor
{
    UIBuilderTestScript manager;
    
    void OnEnable()
    {
        manager = target as UIBuilderTestScript;
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
