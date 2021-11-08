using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBuilderTestScript : MonoBehaviour
{
    public CustomObject obj;
}

[Serializable]
public class CustomObject
{
    public Transform tf;
    public float value;
}
