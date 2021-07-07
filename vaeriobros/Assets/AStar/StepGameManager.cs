using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Assets.Model;

public class StepGameManager : GameManager
{
    string debugLevel = "[[[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,5,2,2,2,2,2,2,2,2,2,2,2],[2,6,7,2,2,2,2,2,2,2,5,2,2,2],[2,8,9,2,2,2,2,2,2,6,7,2,2,2],[2,8,9,2,2,2,2,2,2,8,9,2,2,2],[0,0,0,0,0,0,0,0,0,0,0,0,0,0]],[[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[1,1,1,1,1,1,1,1,1,1,1,1,1,1],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,5,2],[2,2,2,2,2,2,2,2,2,2,2,6,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,9,2],[2,2,2,2,2,2,2,2,2,2,2,8,5,2],[2,2,2,2,2,2,2,2,5,2,2,6,7,2],[2,5,2,2,2,2,2,6,7,2,2,8,9,2],[2,7,2,2,2,2,2,8,9,2,2,8,9,2],[2,9,2,2,2,2,2,8,9,2,2,8,9,2],[0,0,0,0,0,0,0,0,0,0,0,0,0,0]],[[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,5,2,2,2,2,2,2,2,2,2,2,2],[2,2,0,0,0,0,0,0,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[0,0,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,0]],[[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,0,0,0,2,2,2,2,2],[2,2,2,2,2,2,0,0,0,2,2,2,2,2],[2,2,2,2,0,0,0,0,0,2,2,2,2,2],[2,2,2,0,0,0,0,0,0,2,2,2,2,2],[2,2,0,0,0,0,0,0,0,2,2,2,2,2],[2,0,0,0,0,0,0,0,0,2,2,2,2,2],[0,0,0,0,0,0,0,0,0,2,2,2,2,2],[0,0,0,0,0,0,0,0,2,2,2,2,2,2],[0,0,0,0,0,0,0,0,0,0,0,0,0,0]],[[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,1,1,1,1,1,2,2,2],[2,2,2,2,0,0,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,0,0,2,0,0,0,0,0,2,2,2,2]]]";

    protected override void ProcessGameEnd(EndReason reason)
    {

    }

    protected override void ProcessGameStart()
    {
        Physics2D.simulationMode = SimulationMode2D.Script;

        StartCoroutine(DebugPlanner());
    }



    void RunPlan(List<bool[]> actions)
    {
        foreach (bool[] action in actions)
        {
            SingleStep(action);
        }
    }

    protected override void OnStart()
    {
        StartCoroutine(RunSimulation());
    }

    IEnumerator DebugPlanner()
    {
        yield return new WaitForSeconds(2f);
        LevelScene testScene = new LevelScene();
        // read player back
        testScene.plumberXposition = plumber.thisRigidbody.position.x;
        testScene.plumberYposition = plumber.thisRigidbody.position.y;
        testScene.plumberXacceleration = plumber.thisRigidbody.velocity.x;
        testScene.plumberYacceleration = plumber.thisRigidbody.velocity.y;


        List<bool[]> actions = new List<bool[]>();
        for (int i = 0; i < 100; i++)
        {
            actions.Add(AStarSimulator.createAction(false, true, true));
        }
        RunPlan(actions);

        yield return new WaitForSeconds(2f);

        testScene.Tick(AStarSimulator.createAction(false, false, false));
    }

    IEnumerator RunSimulation()
    {
        state = GameState.Loading;
        // clear any existing level
        Time.timeScale = 1;
        followCam.Target = null;
        plumber = null;
        if (procLevel != null)
        {
            procLevel.DestroyLevel();
        }
        List<Chunk> chunks = LoadDebugLevel();
        // create a new level
        procLevel = Instantiate<ProcLevel>(procLevelPrefab, this.transform);
        procLevel.Generate(chunks, false);
        // assign requestId here because this function is called on start
        LoadDebugLevel();
        // link objects in level
        RecurseLinkObjects(procLevel.transform);
        yield return new WaitForEndOfFrame();
        // launch game
        ProcessGameStart();
    }

    List<Chunk> LoadDebugLevel()
    {
        var chunks = new List<Chunk>();

        var ls = JsonConvert.DeserializeObject<List<List<List<int>>>>(debugLevel);
        foreach (var l in ls)
        {
            //make chunk
            var chunk = new Chunk();
            chunk.sizeY = l.Count;
            chunk.sizeX = l.Count;
            foreach (var row in l)
            {
                foreach (var item in row)
                {
                    chunk.blocks.Add((BlockType)item);
                }
            }
            chunks.Add(chunk);
        }
        return chunks;
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
