using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;
using Assets.Model;

class VAECoordConverter : JsonConverter<VAECoord>
{
    public override VAECoord ReadJson(JsonReader reader, Type objectType, VAECoord existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, VAECoord value, JsonSerializer serializer)
    {
        writer.WriteRaw(value.ToString());
    }
}

[Serializable]
public class VAECoord
{
    public float v1;
    public float v2;

    public VAECoord(float v1, float v2)
    {
        this.v1 = v1;
        this.v2 = v2;
    }

    public override string ToString()
    {
        return string.Format("[{0:F},{1:F}]", v1, v2);
    }
}

public enum GameState
{
    Loading,
    Playing,
    Finished
}

public enum EndReason
{
    Win,
    Death,
    Unplayable,
    Boring
}

public abstract class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    protected ProcLevel procLevel;
    [SerializeField] protected FollowCam followCam;
    [SerializeField] protected ProcLevel procLevelPrefab;

    public static float KillHeight {get; private set;}
    public static float MaxHeight { get; private set; }

    [SerializeField] protected FeedbackPanel feedbackPanel;

    protected GameState state;
    protected readonly Dictionary<string, object> levelProviderFields = new Dictionary<string, object>();

    public Player plumber;

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            state = GameState.Finished;
        }
    }

    void OnDestroy()
    {
        if (singleton == this)
        {
            singleton = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        levelProviderFields["playerId"] = Guid.NewGuid();
        levelProviderFields["telemetry"] = new object();
        KillHeight = -5f;
        OnStart();
    }

    protected abstract void OnStart();

    IEnumerator RunRestartGame()
    {
        if(state == GameState.Finished)
        {
            state = GameState.Loading;
            // clear any existing level
            Time.timeScale = 0;
            followCam.Target = null;
            plumber = null;
            if (procLevel != null)
            {
                procLevel.DestroyLevel();
            }
            // create a new level
            procLevel = Instantiate<ProcLevel>(procLevelPrefab, this.transform);
            // assign requestId here because this function is called on start
            levelProviderFields["requestId"] = Guid.NewGuid();           
            yield return DataManager.Instance.ChunksPOST_Coroutine(levelProviderFields);
            var chunks = (List<Chunk>)DataManager.Instance.ProcessedResponse;
            // it used to be so simple...
            // chunks = FetchSampleLevel();
            // generate map
            procLevel.Generate(chunks);
            // link objects in level
            RecurseLinkObjects(procLevel.transform);
            yield return new WaitForEndOfFrame();
            // launch game
            ProcessGameStart();

        }
    }

    protected abstract void ProcessGameStart();
    protected abstract void ProcessGameEnd(EndReason reason);

    internal void SendFinishedSurvey(SurveyResults results,EndReason endReason)
    {
        // get unplayable status
        var unplayable = endReason == EndReason.Unplayable;
        var boring = endReason == EndReason.Boring;
        //Provide data here
        var previousResponse = (LevelProviderResponse)DataManager.Instance.IntermediateResponse;
        levelProviderFields["telemetry"] = new TelemetryData
        {
            latentVectors = previousResponse.LatentVectors,
            levelRepresentation = previousResponse.LevelRepresentation,
            experimentName = previousResponse.ExperimentName,
            //experimentName = "Hey Daniel, Did it work?",
            modelName = "mariovae_z_dim_2",
            markedUnplayable = unplayable,
            endedEarly = boring,
            surveyResults = results
        };
        Restart();
    }


    protected void Restart()
    {
        feedbackPanel.ToggleVisible(false, EndReason.Death);
        StartCoroutine(RunRestartGame());
    }

    protected void RecurseLinkObjects(Transform trans)
    {
        // link components
        var exitFlag = trans.GetComponent<ExitFlag>();
        if (exitFlag != null)
        {
            exitFlag.OnPlayerReachedExit += this.OnPlayerReachedExit;
        }
        var player = trans.GetComponent<Player>();
        if (player != null)
        {
            player.OnDeath += this.OnPlayerDeath;
            followCam.Target = player.transform;
            this.plumber = player;
        }
        // recurse down heirarchy
        for (int i = 0; i < trans.childCount; i++)
        {
            RecurseLinkObjects(trans.GetChild(i));
        }
    }

    #region EVENT_CALLBACKS
    public void OnPlayerReachedExit()
    {
        if (state == GameState.Playing)
        {
            ProcessGameEnd(EndReason.Win);
        }
    }

    public void OnPlayerDeath()
    {
        if (state == GameState.Playing)
        {
            ProcessGameEnd(EndReason.Death);
        }
    }
    #endregion

    public void AbortLevel(EndReason reason)
    {
        if (state == GameState.Playing)
        {
            ProcessGameEnd(reason);
        }
    }


    #region WEBGL_TEST_NETWORKING
    private string CreateJsonRequest()
    {
        var dict = new Dictionary<string, object>();
        var fs = new List<List<float>>() {
            new List<float>() { -6.28f, 6.28f },new List<float>() { 6.28f, -6.28f } };
        dict["experimentName"] = "unity_test";
        dict["zs"] = fs;
        // {"zs":[[0.0,0.0],[1.0,1.0]]}
        return JsonConvert.SerializeObject(dict);
    }

    private List<Chunk> ParseWebResponse(UnityWebRequest request)
    {
        var jsonResponse = request.downloadHandler.text;
        var chunks = new List<Chunk>();

        var ls = JsonConvert.DeserializeObject<List<List<List<int>>>>(jsonResponse);
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
    #endregion

    private List<Chunk> FetchSampleLevel()
    {

        //var coords = new List<VAECoord>() { new VAECoord(0f, 0f), new VAECoord(1, 1) };

        // STUPID JSON!
        var dict = new Dictionary<string, object>();
        var fs = new List<List<float>>() {
            new List<float>() { -6.28f, 6.28f },new List<float>() { 6.28f, -6.28f } };
        dict["experimentName"] = "unity_test";
        dict["zs"] = fs;
        // {"zs":[[0.0,0.0],[1.0,1.0]]}
        var json = JsonConvert.SerializeObject(dict);
        //var json = "{ \"zs\" : [[0.0,0.0],[1.0,1.0]]}";
        //var json = JsonConvert.SerializeObject(dict);
        Debug.Log(json);

        HttpWebRequest request =  (HttpWebRequest)WebRequest.Create("https://mariovae.herokuapp.com/level");
        request.Method = "GET";
        
        /*HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://mariovae.herokuapp.com/level");
        request.Method = "POST";
        request.ContentType = "application/json";
        using (var writer = new StreamWriter(request.GetRequestStream()))
        {
            writer.Write(json);
        }*/

        Debug.Log("SENT!");

        var chunks = new List<Chunk>();
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        using (var reader = new StreamReader(response.GetResponseStream()))
        {
            string jsonResponse = reader.ReadToEnd();
            Debug.Log(jsonResponse);
            var ls = JsonConvert.DeserializeObject<List<List<List<int>>>>(jsonResponse);
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
        }


        // parse response


        return chunks;
    }

}
