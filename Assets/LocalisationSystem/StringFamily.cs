using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StringFamily", menuName = "StringFamily"), Serializable]
public class StringFamily : ScriptableObject
{
    public enum Language
    {
        Spanish = 1,
        English = 2,
        French = 3,
        German = 4,
        Chinese = 5
    }

    public static int NumberOfLanguages => Enum.GetValues(typeof(Language)).Length;
    public Language language;

    public List<StringList> family;

    public void Initialize()
    {
        family = new List<StringList>();

        for (int i = 0; i < 1; i++)
        {
            AddRow();
        }
    }

    public void AddRow()
    {
        family.Add(new StringList());

        for (int i = 0; i < NumberOfLanguages + 1; i++)
        {
            family[family.Count - 1].strings.Add("");//
        }
    }
    
    public void RemoveRow()
    {
        family.RemoveAt(family.Count-1);
    }

    [Serializable]
    public class StringList
    {
        public List<string> strings = new List<string>();
    }

    
}