using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadRotator : MonoBehaviour
{
    IEnumerator Start()
    {
        float frames = 240;

        var initialVector = transform.eulerAngles;
        
        for (int i = 0; i <= frames; i++)
        {
            transform.eulerAngles = 
                Vector3.Lerp(initialVector,
                new Vector3(
                    90,
                    0, 
                    0), i/frames);
            yield return null;
        }
        
        initialVector = transform.eulerAngles;
        
        for (int i = 0; i <= frames; i++)
        {
            transform.eulerAngles = 
                Vector3.Lerp(initialVector,
                    new Vector3(
                        90,
                        180, 
                        0), i/frames);
            yield return null;
        }
        
        initialVector = transform.eulerAngles;
        
        for (int i = 0; i <= frames; i++)
        {
            transform.eulerAngles = 
                Vector3.Lerp(initialVector,
                    new Vector3(
                        0,
                        180, 
                        0), i/frames);
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        // float cameraLookLeftRight = Input.GetAxisRaw("Mouse X");
        // float cameraLookUpDown = Input.GetAxisRaw("Mouse Y");
        //
        // Debug.Log(cameraLookLeftRight + " " + cameraLookUpDown);
        //
        // transform.eulerAngles = 
        //     new Vector3(
        //         transform.eulerAngles.x - cameraLookUpDown,
        //         transform.eulerAngles.y + cameraLookLeftRight, 
        //         transform.eulerAngles.z);
        // transform.eulerAngles = 
        //     new Vector3(
        //         cameraLookUpDown*100,
        //         cameraLookLeftRight*100, 
        //         0);

    }
}
