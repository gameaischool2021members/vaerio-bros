using System.Net;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

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

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    public ProcLevel procLevel;

    private void Awake()
    {
        if (singleton != null)
        {
            singleton = this;
        }
    }

    private void OnDestroy()
    {
        if(singleton == this)
        {
            singleton = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DoIt();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void DoIt()
    {
        // receive level description
        var chunks = FetchSampleLevel();
        //var chunks = new List<Chunk>();
        // generate map

        procLevel.Generate(chunks);
        // link objects in level
        RecurseLinkObjects(procLevel.transform);
        // launch game

        // wait for game over signal
    }

    void RecurseLinkObjects(Transform trans)
    {
        var exitFlag = trans.GetComponent<ExitFlag>();
        if (exitFlag != null)
        {
            exitFlag.OnPlayerReachedExit += this.OnPlayerReachedExit;
        }
        var player = trans.GetComponent<Player>();
        if (player != null)
        {
            player.OnDeath += this.OnPlayerDeath;
        }


        for (int i=0;i< trans.childCount; i++)
        {
            RecurseLinkObjects(trans.GetChild(i));
        }
    }

    #region EVENT_CALLBACKS
    public void OnPlayerReachedExit()
    {
        Debug.Log("PLAYER REACHED EXIT");
    }

    public void OnPlayerDeath()
    {
        Debug.Log("PLAYER DEAD");
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


        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://mariovae.herokuapp.com/level");
        request.Method = "POST";
        request.ContentType = "application/json";
        using (var writer = new StreamWriter(request.GetRequestStream()))
        {
            writer.Write(json);
        }
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
