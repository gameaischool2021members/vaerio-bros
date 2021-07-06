using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SharedData
{

    public static float runSpeed = 5;
    public static float acceleration = 10;
    public static float jumpPower = 13;
    public static float bouncePower = 6;


    /// <summary>
    /// DOES NOT INCLUDE GRAVITY CALCULATIONS
    /// </summary>
    /// <param name="currentVel"></param>
    /// <param name="walkInput">-1 to 1</param>
    /// <param name="jumpInput">0 to 1</param>
    /// <returns></returns>
    public static Vector2 ComputePlayerVelocity(Vector2 currentVel, float walkInput, float jumpInput, float timestep)
    {
        // update desired velocity
        var desiredVelX = walkInput * runSpeed;

        // assign to actual vector
        currentVel.x = Mathf.MoveTowards(currentVel.x, desiredVelX, acceleration * timestep);
        bool canJump = true;
        if (canJump && jumpInput > 0)
        {
            currentVel.y = jumpPower;
        }
        return currentVel;
    }

}
