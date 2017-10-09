using UnityEngine;
using System.Collections;
using Vox.Core;
using Vox;
using Vox.Core.Blocks;
using Vox.Database;

public class Test : MonoBehaviour
{
    private Chunk _chunk;

    void Start()
    {
        Random.InitState(0);
        _chunk = new Chunk();
        _chunk.SetBlockType(1, new ColoredBlock(new BlockConfig { Type = 1, Name = "CB", Solid = true }));
        _chunk.SetBlockType(0, new Block(new BlockConfig { Type = 0, Name = "Air" }));
        for (var x = 0; x < Env.ChunkSize; x++)
        {
            for (var y = 0; y < Env.ChunkSize; y++)
            {
                for (var z = 0; z < Env.ChunkSize; z++)
                {
                    if (Random.value > 0.7)
                    {
                        _chunk.SetBlockData(x, y, z, new BlockData(1, true));
                    }
                }
            }
        }

        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
        _chunk.RenderGeometryHandler.Build();
        sw.Stop();
        Debug.LogFormat("{0}", sw.ElapsedMilliseconds);
        _chunk.RenderGeometryHandler.Commit();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
