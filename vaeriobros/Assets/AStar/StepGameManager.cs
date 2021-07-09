using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Assets.Model;

public struct GapInfo
{
    public float LeftHeight { get; private set; }
    public float RightHeight { get; private set; }
    public float MinHeight { get; }
    public float MaxHeight { get; }

    public GapInfo(float leftHeight, float rightHeight)
    {
        LeftHeight = leftHeight;
        RightHeight = rightHeight;
        MinHeight = Mathf.Min(LeftHeight, RightHeight);
        MaxHeight = Mathf.Max(LeftHeight, RightHeight);
    }

    public override string ToString()
    {
        return string.Format("Gap({0:F},{1:F})", LeftHeight, RightHeight);
    }

}

public class StepGameManager : GameManager
{
    string debugLevel = "[[[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,5,2,2,2,2,2,2,2,2,2,2,2],[2,6,7,2,2,2,2,2,2,2,5,2,2,2],[2,8,9,2,2,2,2,2,2,6,7,2,2,2],[2,8,9,2,2,2,2,2,2,8,9,2,2,2],[0,0,0,0,0,0,0,0,0,0,0,0,0,0]],[[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[1,1,1,1,1,1,1,1,1,1,1,1,1,1],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,5,2],[2,2,2,2,2,2,2,2,2,2,2,6,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,9,2],[2,2,2,2,2,2,2,2,2,2,2,8,5,2],[2,2,2,2,2,2,2,2,5,2,2,6,7,2],[2,5,2,2,2,2,2,6,7,2,2,8,9,2],[2,7,2,2,2,2,2,8,9,2,2,8,9,2],[2,9,2,2,2,2,2,8,9,2,2,8,9,2],[0,0,0,0,0,0,0,0,0,0,0,0,0,0]],[[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,5,2,2,2,2,2,2,2,2,2,2,2],[2,2,0,0,0,0,0,0,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[0,0,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,0]],[[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,0,0,0,2,2,2,2,2],[2,2,2,2,2,2,0,0,0,2,2,2,2,2],[2,2,2,2,0,0,0,0,0,2,2,2,2,2],[2,2,2,0,0,0,0,0,0,2,2,2,2,2],[2,2,0,0,0,0,0,0,0,2,2,2,2,2],[2,0,0,0,0,0,0,0,0,2,2,2,2,2],[0,0,0,0,0,0,0,0,0,2,2,2,2,2],[0,0,0,0,0,0,0,0,2,2,2,2,2,2],[0,0,0,0,0,0,0,0,0,0,0,0,0,0]],[[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,1,1,1,1,1,2,2,2],[2,2,2,2,0,0,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,2,2,2,2,2,2,2,2,2,2,2,2],[2,2,0,0,2,0,0,0,0,0,2,2,2,2]]]";


    [SerializeField] bool showGapDebug = false;
    public GapInfo[] gapData = null;
    public float[] floorHeights = null;

    public List<Vector2> currentPlan; // the current plan positions for Drawing Gizmos

    public float speedup = 1;


    protected override void ProcessGameEnd(EndReason reason)
    {

    }

    protected override void ProcessGameStart()
    {
        Physics2D.simulationMode = SimulationMode2D.Script;

        // StartCoroutine(DebugPlanner());
        // StartCoroutine(Solve());
        StartCoroutine(Optimize());
    }


    /// <summary>
    /// run the plan. Repetitions is the number of steps we repeated during planning
    /// </summary>
    /// <param name="actions"></param>
    /// <param name="repetitions"></param>
    /// <returns></returns>
    IEnumerator RunPlan(List<bool[]> actions, int repetitions)
    {
        foreach (bool[] action in actions)
        {
            for (int i = 0; i < repetitions; i++)
            {
                yield return new WaitForSeconds(0.02f);
                SingleStep(action);
            }
        }
    }

    protected override void OnStart()
    {
        StartCoroutine(RunSimulation());
    }

    IEnumerator Optimize()
    {
        AStarSimulator simulator = new AStarSimulator();
        while (true)
        {
            bool[] action = simulator.optimise();
            currentPlan = simulator.currentPlanPositions;
            // bool[] action = AStarSimulator.createAction(false, true, false);
            SingleStep(action);
            yield return new WaitForSeconds(0.02f);
        }
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
        StartCoroutine(RunPlan(actions, 1));

        yield return new WaitForSeconds(2f);

        testScene.Tick(AStarSimulator.createAction(false, false, false));
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 1f);
        foreach (Vector2 planPosition in currentPlan)
        {
            Gizmos.DrawSphere(planPosition, 0.2f);
        }

        /* the bool is not working for me
        if (showGapDebug)
        {
            if (gapData == null)
            {
                return;
            }
            Gizmos.color = new Color(0, 1, 0, 0.5f);

            for (int i = 0; i < gapData.Length; i++)
            {
                var gap = gapData[i];
                if (gap.MaxHeight > 0)
                {
                    var center = new Vector3(i, -0.5f + gap.MinHeight / 2, 0);
                    var size = new Vector3(1, gap.MinHeight, 0.1f);
                    Gizmos.DrawCube(center, size);
                }
            }

            Gizmos.color = new Color(0, 0, 1, 0.5f);
            for (int i = 0; i < gapData.Length; i++)
            {
                var gap = gapData[i];
                if (gap.MaxHeight > 0)
                {
                    var center = new Vector3(i, -0.5f + gap.MaxHeight / 2, 0);
                    var size = new Vector3(1, gap.MaxHeight, 0.1f);
                    Gizmos.DrawCube(center, size);
                }
            }

            if (floorHeights == null)
            {
                return;
            }

            Gizmos.color = new Color(1, 0, 0, 0.5f);
            for (int i = 0; i < floorHeights.Length; i++)
            {
                var height = floorHeights[i];
                if (height > 0)
                {
                    var center = new Vector3(i, -0.5f + height / 2, 0);
                    var size = new Vector3(1, height, 0.1f);
                    Gizmos.DrawCube(center, size);
                }
            }
        }
        */
    }

    public void AnalyseLevel()
    {
        gapData = new GapInfo[procLevel.LevelWidth];
        floorHeights = new float[procLevel.LevelWidth];
        for (int i = 0; i < procLevel.LevelWidth; i++)
        {
            Vector2 floorPos = new Vector2(i, 0);
            Vector2 ceilingPos = new Vector2(i, procLevel.LevelHeight);

            // check occupancy
            var obs = Physics2D.OverlapPoint(floorPos, SharedData.SolidLayers);
            if (obs != null)
            {
                floorHeights[i] = 0;
                continue;
            }
            else
            {
                floorHeights[i] = Mathf.Infinity;
                gapData[i] = AnalyseGap(floorPos, procLevel.LevelHeight, procLevel.ChunkWidth);
                // Debug.Log("Gap at " + floorPos + " = " + gapData[i]);
            }
            // linecast upwards
            var hit = Physics2D.Linecast(floorPos, ceilingPos, SharedData.SolidLayers);
            // only measure a "gap" if there is no ceiling
            if (hit.collider == null)
            {
                floorHeights[i] = Mathf.Infinity;
                gapData[i] = AnalyseGap(floorPos, procLevel.LevelHeight, procLevel.ChunkWidth);
                Debug.Log("Gap at " + floorPos + " = " + gapData[i]);
            }
            else
            {
                floorHeights[i] = Mathf.CeilToInt(hit.distance);
            }
        }
    }

    private GapInfo AnalyseGap(Vector2 floorPos, int levelHeight, int chunkWidth)
    {
        //const float HALF_TILE_HEIGHT = 0.5f;
        int closestLeft = chunkWidth;
        int closestRight = chunkWidth;

        bool needLeft = true;
        float leftHeight = 0;
        bool needRight = true;
        float rightHeight = 0;
        for (int i = 0; i < levelHeight; i++)
        {
            // calculate end points
            var midPos = new Vector2(floorPos.x, i);
            var leftPos = new Vector2(floorPos.x - closestLeft, i);
            var rightPos = new Vector2(floorPos.x + closestRight, i);
            // check left
            if (needLeft)
            {
                var leftHit = Physics2D.Linecast(midPos, leftPos, SharedData.SolidLayers);
                if (leftHit.collider == null)
                {
                    leftHeight = i;
                    needLeft = false;
                }
                else
                {
                    var dist = Mathf.CeilToInt(leftHit.distance);
                    if (dist < closestLeft)
                    {
                        closestLeft = dist;
                    }
                }
            }
            // check right
            if (needRight)
            {
                var rightHit = Physics2D.Linecast(midPos, rightPos, SharedData.SolidLayers);
                // if we hit nothing, we found the minimum height
                if (rightHit.collider == null)
                {
                    rightHeight = i;
                    needRight = false;
                }
                else
                {
                    var dist = Mathf.CeilToInt(rightHit.distance);
                    if (dist < closestRight)
                    {
                        closestRight = dist;
                    }
                }
            }


        }
        return new GapInfo(leftHeight, rightHeight);
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
        // link objects in level
        RecurseLinkObjects(procLevel.transform);
        yield return new WaitForEndOfFrame();
        AnalyseLevel();
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
        Physics2D.Simulate(speedup * Time.fixedDeltaTime);
        Physics2D.SyncTransforms();
    }
}
