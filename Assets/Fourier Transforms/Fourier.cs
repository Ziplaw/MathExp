using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fourier : MonoBehaviour
{
    public Palo[] palos;
    public Transform obj;

    private void Update()
    {
        float deltaTime = Time.time * 360;
        
        palos[0].pos = Vector3.zero;
        palos[0].rot = Quaternion.Euler(0, 0, deltaTime);
        palos[0].scale = 25;
        
        for (int i = 1; i < palos.Length; i++)
        {
            palos[i].pos = palos[i - 1].pos + palos[i - 1].rot * Vector3.right * palos[i].scale;
            palos[i].rot = palos[i - 1].rot * Quaternion.Euler(0, 0, deltaTime * (i % 2 == 0 ? 1 : -1));
            palos[i].scale = palos[i - 1].scale * .5f;
        }
        
        obj.position = new Vector3(5, palos.Last().pos.y,Time.time);
    }

    private void OnDrawGizmos()
    {
        for (int i = 1; i < palos.Length; i++)
        {
            float col = i / (float) palos.Length;
            Gizmos.color = new Color(col,0,0,1);
            Gizmos.DrawLine(palos[i].pos, palos[i-1].pos);
        }
    }
[Serializable]
    public struct Palo
    {
        public Vector3 pos;
        public Quaternion rot;
        public float scale;
    }
}
