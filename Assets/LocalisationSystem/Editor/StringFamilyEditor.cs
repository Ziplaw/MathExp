using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StringFamily))]
public class StringFamilyEditor : Editor
{
    StringFamily manager;
    
    void OnEnable()
    {
        manager = target as StringFamily;
    }
    
    public override void OnInspectorGUI()
    {
        if (manager.family == null)
        {
            if (GUILayout.Button("Initialize"))
            {
                manager.Initialize();
            }
        }
        else
        {
            manager.language = (StringFamily.Language)GUILayout.Toolbar((int)manager.language, new[] {"ID", "Spanish", "English", "French", "German","Chinese"});
            for (int i = 0; i < manager.family.Count; i++)
            {
                using (new GUILayout.HorizontalScope())
                {
                    for (int j = 0; j < manager.family[i].strings.Count; j++)
                    {
                        var prev = manager.family[i].strings[j];
                        manager.family[i].strings[j] =
                            EditorGUILayout.TextField(manager.family[i].strings[j]);
                        if (manager.family[i].strings[j] != prev) EditorUtility.SetDirty(this);
                    }
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("+"))
                {
                    manager.AddRow();
                }
                if (GUILayout.Button("-"))
                {
                    manager.RemoveRow();
                }
            }
        }
    }
}
