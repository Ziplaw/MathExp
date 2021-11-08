using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleController : MonoBehaviour
{
    public Transform planet;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        rb.AddForce(transform.forward * movement.y);
        RotateUsingVector();
        // RotateUsingQuaternion();

        rb.AddForce(planet.position - transform.position);
    }

    private void RotateUsingQuaternion()
    {
        var up = (transform.position - planet.position).normalized;
        transform.rotation = Quaternion.FromToRotation(transform.up, up) * transform.rotation;
    }

    void RotateUsingVector()
    {
        transform.up = (transform.position - planet.position).normalized;

    }
    
}
