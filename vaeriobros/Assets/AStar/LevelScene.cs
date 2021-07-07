using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class to hold the data of the levelScene which is relevant for the A* solver
/// I thought we could load the layout of the Scene in a seperate Unity Scene for simulations
/// We ignore Enemies and fireballs for simplicity 
/// </summary>
public class LevelScene : MonoBehaviour
{
    public float plumberXposition; // mario.x in the original
    public float plumberYposition;
    public float plumberXacceleration; // I think that ist mario.xa in the original
    public int plumberDamage = 0; // the damage(falling is counting as damage) that the plumber is getting (Why do we need to store this?)

    public LevelScene Clone()
    {
        LevelScene clonedScene = new LevelScene();
        clonedScene.plumberXposition = this.plumberXposition;
        clonedScene.plumberYposition = this.plumberYposition;
        clonedScene.plumberXacceleration = this.plumberXacceleration;
        clonedScene.plumberDamage = this.plumberDamage;
        
        return clonedScene;
    }

    /// <summary>
    /// This function should do one update of the game scene and update the current LevelScene
    /// Should take an action, best in the form of a boolean array bool[] with true for each action(actually button) from the Actions enum
    /// </summary>
    public void Tick(bool[] action)
    {
        // TODO
    }

    /// <summary>
    /// this should return wheter the current position is a gap
    /// The original uses a discrete reperesentation of the level here
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool isGap( float position)
    {
        // TODO
        return true;
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
        return 0;
    }

    /// <summary>
    /// can the plumber jump in the current scene?
    /// </summary>
    public bool PlumberMayJump()
    {
        return true;
    }
}

public enum Actions
{
    Jump,
    Left,
    Right
}