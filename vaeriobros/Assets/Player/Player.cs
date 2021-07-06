using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event Action OnDeath;


    Rigidbody2D thisRigidbody;

    [SerializeField] float runSpeed = 5;
    [SerializeField] float acceleration = 10;
    [SerializeField] float jumpPower = 10;
    [SerializeField] float bouncePower = 4;

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
        // update desired velocity
        var desiredVelX = walkInput * runSpeed;

        // assign to actual vector
        var currentVel = thisRigidbody.velocity;
        currentVel.x = Mathf.MoveTowards(currentVel.x, desiredVelX, acceleration * Time.deltaTime);
        bool canJump = true;
        if (canJump && jumpInput > 0)
        {
            currentVel.y = jumpPower;
        }
        thisRigidbody.velocity = currentVel;
    }
}
