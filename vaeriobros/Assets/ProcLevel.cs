using Assets.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum BlockType
{
    Wall,
    Brick,
    None,
    Item,
    Down,
    Goomba,
    PipeUL,
    PipeUR,
    PipeL,
    PipeR,
    Coin

}

public class ProcLevel : MonoBehaviour
{
    [SerializeField] GameObject plumberPrefab;
    [SerializeField] GameObject exitPrefab;
    [SerializeField] List<GameObject> prefabs;

    public void Generate(List<Chunk> chunks)
    {
        int startX = 14;
        // add starting padding
        for (int i = 0; i < startX; i++)
        {
            InstanceBlock(i, 0, BlockType.Wall);
        }
        // add spawn
        InstanceSpawn(3, 2);

        int endX = startX;
        // spawn level sections
        for(int c = 0; c < chunks.Count; c++)
        {
            var chunk = chunks[c];
            

            for(int i = 0; i < chunk.blocks.Count; i++)
            {
                var block = chunk.blocks[i];
                var x = i % chunk.sizeX;
                var row = i / chunk.sizeY;
                var y = chunk.sizeY - row - 1;
                InstanceBlock(endX+x, y, block);
            }
            endX += chunk.sizeX;
        }
        // add finishing padding
        for(int i = 0; i < 5; i++)
        {
            var x = endX + i;
            InstanceBlock(x, 0, BlockType.Wall);
            
        }
        InstanceExit(endX + 3, 1);
    }

    internal void DestroyLevel()
    {
        RecurseRemoveObjects(transform);
    }

    void RecurseRemoveObjects(Transform trans)
    {
        Destroy(trans.gameObject);
        for(int i = 0; i < trans.childCount; i++)
        {
            RecurseRemoveObjects(trans.GetChild(i));
        }
    }

    private void InstanceExit(int x, int y)
    {
        var obj = Instantiate<GameObject>(exitPrefab, new Vector3(x, y, 0), Quaternion.identity, this.transform);
    }

    private void InstanceSpawn(int x, int y)
    {
        var obj = Instantiate<GameObject>(plumberPrefab, new Vector3(x, y, 0), Quaternion.identity, this.transform);
    }

    private void InstanceBlock(int x, int y, BlockType block)
    {
        if(block == BlockType.None)
        {
            return;
        }
        var index = (int)block;
        var obj = Instantiate<GameObject>(prefabs[index], new Vector3(x, y, 0), Quaternion.identity,this.transform);
    }
}
