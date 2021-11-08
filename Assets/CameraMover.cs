using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public List<Vector3> points;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        transform.position = points[0];

        for (int i = 0; i <= 1080; i++)
        {
            transform.position = Vector3.Lerp(transform.position, points[1], .01f);
            yield return null;
        }
        
        transform.SetParent(GameObject.Find("Capsule").transform);
        
    }

    
    
    
    
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        foreach (var point in points)
        {
            Gizmos.DrawWireSphere(point, 1f);
        }
    }
}
