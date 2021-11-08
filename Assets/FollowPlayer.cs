using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;

    public bool inside;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inside = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inside = false;
        }
    }
    
    private Vector2 dir;

    private void Update()
    {
        dir = player.position - transform.position;
        transform.up = Vector2.Lerp(transform.up, dir.normalized, .025f);
        
        ExtendTentacle();
    }

    
    [ContextMenu("Rotate")]
    public void ExtendTentacle()
    {
        //rotate
        if (inside)
        {
            // player.GetHit();
        }
        //test for the player being inside the tentacle's hitbox
        //hit the player
    }
}
