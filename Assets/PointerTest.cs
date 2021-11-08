using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerTest : MonoBehaviour
{
    public Vector2 movement;
    private Pointered thing;

    private void Start()
    {
        thing = new Pointered(ref movement);
    }

    private void Update()
    {
        thing.Update();
    }
}

public class Pointered
{
    private Vector2 movement;

    public Pointered(ref Vector2 movement)
    {
        this.movement = movement;
    }

    public void Update()
    {
        Debug.Log(movement);
    }
}
