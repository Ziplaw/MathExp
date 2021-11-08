using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MyLabel))]
public class LabelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}

[CustomEditor(typeof(CETest))]
public class CETestEditor : Editor
{
    private MyLabel labelEditor;
    private void OnEnable()
    {
        labelEditor = FindObjectOfType<MyLabel>();
    }

    public override void OnInspectorGUI()
    {
        CreateEditor(labelEditor);
        base.OnInspectorGUI();
    }
}
