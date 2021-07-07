using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepGameManager : GameManager
{
    protected override void ProcessGameEnd(EndReason reason)
    {
        
    }

    protected override void ProcessGameStart()
    {
        Physics2D.simulationMode = SimulationMode2D.Script;
    }

    /// <summary>
    ///  do a single step of the environment
    /// </summary>
    /// <param name="action"></param>
    public void SingleStep(bool[] action)
    {
        plumber.jumpInput = 0;
        plumber.walkInput = 0;
        if (action[(int)Actions.Left])
        {
            plumber.walkInput = -1;
        }
        if (action[(int)Actions.Right])
        {
            plumber.walkInput = 1;
        }
        if (action[(int)Actions.Jump])
        {
            plumber.jumpInput = 1;
        }
        plumber.PhysiscsStep(); // TODO does this need to happen before or after the simulation?
        Physics2D.Simulate(Time.fixedDeltaTime);
    }
}
