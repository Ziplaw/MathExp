using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ScriptChanger : MonoBehaviour
{
    [MenuItem("Tools/create")]
    public static void Create()
    {
        string path = Directory.GetFiles(Application.dataPath, "*CGtestScript*", SearchOption.AllDirectories)[0];

        string text = File.ReadAllText(path);

        var texts = text.SplitAndReAdd("TestMethodToAddEventInvoke");

        var prev = texts[0];
        var next = texts[1];

        var textToAppendTo = next.Split('}')[0];
        textToAppendTo += @"
        string boy;
    ";
        var nextList = next.Split('}').ToList();
        nextList.RemoveAt(0);
        
        var restOfTheScript = string.Join("}", nextList);

        var latterHalf = string.Join("}", textToAppendTo, restOfTheScript);

        var result = prev + latterHalf;
        
        Debug.Log(result);

    }

    
}

public static class Extensions
{

    public static string[] SplitAndReAdd(this string input, string separator)
    {
        var array = input.Split(new string[] {separator}, StringSplitOptions.None);
        for (var i = 0; i < array.Length; i++)
        {
            array[i] += separator;
        }

        return array;
    }
}
