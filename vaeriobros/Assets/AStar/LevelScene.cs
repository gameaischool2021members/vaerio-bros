using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class to hold the data of the levelScene which is relevant for the A* solver
/// I thought we could load the layout of the Scene in a seperate Unity Scene for simulations
/// We ignore Enemies and fireballs for simplicity 
/// </summary>
public class LevelScene
{
    public float plumberXposition; // mario.x in the original
    public float plumberYposition;
    public float plumberXacceleration; // I think that ist mario.xa in the original
    public float plumberYacceleration;
    public int plumberDamage = 0; // the damage(falling is counting as damage) that the plumber is getting (Why do we need to store this?)

    public LevelScene Clone()
    {
        LevelScene clonedScene = new LevelScene();
        clonedScene.plumberXposition = this.plumberXposition;
        clonedScene.plumberYposition = this.plumberYposition;
        clonedScene.plumberXacceleration = this.plumberXacceleration;
        clonedScene.plumberYacceleration = this.plumberYacceleration;
        clonedScene.plumberDamage = this.plumberDamage;
        
        
        return clonedScene;
    }

    /// <summary>
    /// This function should do one update of the game scene and update the current LevelScene
    /// Should take an action, best in the form of a boolean array bool[] with true for each action(actually button) from the Actions enum
    /// </summary>
    public void Tick(bool[] action)
    {
        // teleport player to position
        StepGameManager stepGameManager = (StepGameManager)GameManager.singleton;
        if (stepGameManager == null)
        {
            Debug.LogError("Wrong Game Manager");
        }
        Player plumber = stepGameManager.plumber;
        plumber.thisRigidbody.position = new Vector3(plumberXposition, plumberYposition, plumber.transform.position.z);
        plumber.thisRigidbody.velocity = new Vector2(plumberXacceleration, plumberYacceleration);
        plumber.thisRigidbody.angularVelocity = 0;
        Physics2D.SyncTransforms(); // TODO maybe move into SingleStep()

        stepGameManager.SingleStep(action);

        // read player back
        GetCurrentScene();
    }

    /// <summary>
    /// this should return wheter the current position is a gap
    /// The original uses a discrete reperesentation of the level here
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool isGap( float position, float y_position)
    {
        int roundedPosition = Mathf.RoundToInt(position);
        StepGameManager stepGameManager = (StepGameManager)GameManager.singleton;

        //Dirty fix for not going left.
        if (position < 0f)
        {
            return true;
        }

        float floorHeight = stepGameManager.floorHeights[roundedPosition];



        return y_position < floorHeight;
    }

    /// <summary>
    /// this should return the Height of the gap
    /// if plumbers y position is HIGHER (WTF) then the plumber is falling through the ground
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public float gapHeight( float position)
    {
        // TODO
        int roundedPosition = Mathf.RoundToInt(position);
        StepGameManager stepGameManager = (StepGameManager)GameManager.singleton;

        //Dirty fix for not going left.
        if (position < 0f)
        {
            return Mathf.Infinity;
        }

        float minHeight = stepGameManager.gapData[roundedPosition].MinHeight;

        Debug.Log("position" + position); 
        Debug.Log("MinHeight" + minHeight);

        return minHeight;
    }

    /// <summary>
    /// can the plumber jump in the current scene?
    /// </summary>
    public bool PlumberMayJump()
    {
        Player plumber = GetPlumber();
        RestorePlumberPosition(plumber);

        return plumber.ForcePhysCheckGround() && plumberYacceleration <= 0; ;
    }

    public void RestoreThisScene()
    {
        Player plumber = GetPlumber();
        RestorePlumberPosition(plumber);
    }

    public void RestorePlumberPosition(Player _plumber)
    {
        _plumber.thisRigidbody.position = new Vector3(plumberXposition, plumberYposition, _plumber.transform.position.z);
        _plumber.thisRigidbody.velocity = new Vector2(plumberXacceleration, plumberYacceleration);
        _plumber.thisRigidbody.angularVelocity = 0;
        Physics2D.SyncTransforms(); // TODO maybe move into SingleStep()
    }

    public Player GetPlumber()
    {
        // teleport player to position
        StepGameManager stepGameManager = (StepGameManager)GameManager.singleton;
        if (stepGameManager == null)
        {
            Debug.LogError("Wrong Game Manager");
        }
        return stepGameManager.plumber;
    }

    public void GetCurrentScene()
    {
        Player plumber = GetPlumber();
        this.plumberXposition = plumber.thisRigidbody.position.x;
        this.plumberYposition = plumber.thisRigidbody.position.y;
        this.plumberXacceleration = plumber.thisRigidbody.velocity.x;
        this.plumberYacceleration = plumber.thisRigidbody.velocity.y;
    }
}

public enum Actions
{
    Jump,
    Left,
    Right
}