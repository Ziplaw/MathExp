using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class AniCurveDemo : MonoBehaviour
{
    public Vector2 pos1, pos2;
    public AnimationCurve curve;
    public float strength;
    public int resolution = 100;

    private void Start()
    {
        OnDeath();
    }

    IEnumerator MoveBaby(Vector3 origin, Vector3 target, int frames, AnimationCurve curve, float altitude)
    {
        Debug.Log("hi");

        for (int i = 0; i <= frames; i++)
        {
            float t = i / (float) frames;
            transform.position = Vector2.Lerp(origin, target, t) + Vector2.up * curve.Evaluate(t) * altitude;
            yield return new WaitForEndOfFrame();
        }
        
    }
    
    void OnDeath()
    {
        
        
        StartCoroutine(MoveBaby( pos1, transform.position + (Vector3)pos2, 60, curve, strength));
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(pos1,.1f);
        Gizmos.DrawSphere(transform.position + (Vector3)pos2,.1f);
    }
}
