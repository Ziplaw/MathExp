using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    private Vector2 movement;
    
    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
        
        transform.Translate(new Vector3(0,0,movement.y*Time.deltaTime));
        transform.Rotate(Vector3.up,movement.x);
    }
}
