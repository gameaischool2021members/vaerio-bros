using Assets.DataLayer;
using Assets.DataLayer.Interfaces;
using Assets.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public IGenericRequester Requester { get; private set; }

    [HideInInspector]
    public object ProcessedResponse;

    [HideInInspector]
    public object IntermediateResponse;

    private void Awake()
    {
        Instance = this;
        Requester = new GenericRequester("https://vaerio-level-providor.herokuapp.com");
    }

    void ParseChunks(LevelProviderResponse response)
    {
        IntermediateResponse = response;
        var ls = response.LevelRepresentation;
        var chunks = new List<Chunk>();
        foreach (var l in ls)
        {
            //make chunk
            var chunk = new Chunk();
            chunk.sizeY = l.Count;
            chunk.sizeX = l[0].Count;
            foreach (var row in l)
            {
                foreach (var item in row)
                {
                    chunk.blocks.Add((BlockType)item);
                }
            }
            chunks.Add(chunk);
        }
        ProcessedResponse = chunks;
    }

    public IEnumerator ChunksPOST_Coroutine(Dictionary<string, object> keyValues)
    {
        yield return Requester.PostObject<Dictionary<string, object>, LevelProviderResponse>(keyValues, "level", ParseChunks);
    }

    public IEnumerator ChunksGET_Coroutine()
    {
        yield return Requester.GetObject<LevelProviderResponse>("level", ParseChunks);
    }
}
