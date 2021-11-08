using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class LocalisedString
{
    public StringFamily stringFamily;
    public int ID;

    public static implicit operator string(LocalisedString localisedString)
    {
        return localisedString.stringFamily.family[localisedString.ID].strings[(int) localisedString.stringFamily.language];
    }
}


