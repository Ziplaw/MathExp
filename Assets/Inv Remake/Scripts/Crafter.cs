using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Crafter : MonoBehaviour
{
    public List<Item> inputItems;
    public List<Recipe> allRecipes;
    [Space] public Item output;
    
    List<Item> sortedInput => (from i in inputItems orderby i.name select i).ToList();

    List<Recipe> sortedRecipes
    {
        get
        {
            List<Recipe> sorted = new List<Recipe>();
            for (int j = 0; j < allRecipes.Count; j++)
            {
                sorted.Add(ScriptableObject.CreateInstance<Recipe>());
                sorted[j].items = (from i in allRecipes[j].items orderby i.name select i).ToArray();
                sorted[j].output = allRecipes[j].output;
            }
            return sorted;
        }
    }
    public Item Craft()
    {
        Item output = null;
        sortedRecipes.ForEach(recipe =>
        {
            if (Equals(recipe.items.ToList(),sortedInput))
            {
                output = recipe.output;
            }
        });

        Debug.LogWarning("Crafteando Objeto: " + output);
        return output;
    }
    
    static bool Equals<T> (List<T> a, List<T> b) {
        if (a == null) return b == null;
        if (b == null || a.Count != b.Count) return false;
        for (int i = 0; i < a.Count; i++) {
            if (!object.Equals(a[i], b[i])) {
                return false;
            }
        }
        return true;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(Crafter))]
public class CrafterEditor : Editor
{
    private Crafter manager;

    private void OnEnable()
    {
        manager = target as Crafter;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Attempt Craft"))
        {
            manager.output = manager.Craft();
        }
    }
}

#endif


