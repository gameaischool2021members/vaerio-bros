using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event Action OnDeath;


    Rigidbody2D thisRigidbody;

    

    float walkInput;
    float jumpInput;
    Vector2 vel;
    // Start is called before the first frame update
    void Start()
    {
        thisRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        walkInput = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetAxisRaw("Vertical");
        jumpInput = Mathf.Clamp01(jumpInput);
    }

    void FixedUpdate()
    {
        var currentVel = thisRigidbody.velocity;

        currentVel = SharedData.ComputePlayerVelocity(currentVel, walkInput, jumpInput, Time.deltaTime);

        thisRigidbody.velocity = currentVel;
    }
}
