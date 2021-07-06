using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerEnemy : MonoBehaviour
{
    [SerializeField] Vector2 walkDir = Vector2.left;
    [SerializeField] float walkSpeed = 2;
    [SerializeField] float sensorDist = 0.46f;
    [SerializeField] LayerMask solidCheckLayers;

    Rigidbody2D thisRigidbody;
    // Start is called before the first frame update
    void Start()
    {
        thisRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        var checkPoint = thisRigidbody.position + (walkDir * sensorDist);
        var col = Physics2D.OverlapPoint(checkPoint, solidCheckLayers);
        var turnAround = col != null;

        if (turnAround)
        {
            walkDir = -walkDir;
        }
        var vel = thisRigidbody.velocity;
        vel.x = walkDir.x * walkSpeed;
        thisRigidbody.velocity = vel;
    }

    internal void Squished()
    {
        Destroy(gameObject);
    }
}
