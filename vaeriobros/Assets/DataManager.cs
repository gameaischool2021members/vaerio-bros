using Assets.DataLayer;
using Assets.DataLayer.Interfaces;
using Assets.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public IGenericRequester chunkRequester { get; private set; }

    [HideInInspector]
    public object ResponseDynamic;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(TestingCoroutine());
    }

    private void Awake()
    {
        Instance = this;
        chunkRequester = new GenericRequester("https://mariovae.herokuapp.com");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ParseChunks(List<List<List<int>>> ls)
    {
        var chunks = new List<Chunk>();
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
        ResponseDynamic = chunks;
    }

    public List<Chunk> ChunksPOST(Dictionary<string, object> keyValues)
    {
        StartCoroutine(ChunksPOST_Coroutine(keyValues));
        return (List<Chunk>)ResponseDynamic;
    }

    public List<Chunk> ChunksGET()
    {
        StartCoroutine(ChunksGET_Coroutine());
        return (List<Chunk>)ResponseDynamic;
    }

    public IEnumerator ChunksPOST_Coroutine(Dictionary<string, object> keyValues)
    {
        yield return chunkRequester.PostObject<Dictionary<string, object>, List<List<List<int>>>>(keyValues, "level", ParseChunks);
        //yield return new WaitForSeconds(10f);
    }

    public IEnumerator ChunksGET_Coroutine()
    {
        yield return chunkRequester.GetObject<List<List<List<int>>>>("level", ParseChunks);
        //yield return new WaitForSeconds(10f);
    }
}
