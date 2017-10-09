using UnityEngine;
using System.Collections.Generic;
using g3;
using Vox.Utils;

namespace Vox.Core
{
    public class World
    {
        private Dictionary<Vector3i, Chunk> _chunks = new Dictionary<Vector3i, Chunk>();

        public Chunk GetChunk(Vector3 pos)
        {
            return GetChunk(pos.x, pos.y, pos.z);
        }

        public Chunk GetChunk(float x, float y, float z)
        {
            var p = new Vector3i(Helpers.MakeChunkCoordinate(Mathf.FloorToInt(x)),
                                 Helpers.MakeChunkCoordinate(Mathf.FloorToInt(y)),
                                 Helpers.MakeChunkCoordinate(Mathf.FloorToInt(z)));
            Chunk c = null;
            _chunks.TryGetValue(p, out c);
            return c;
        }
    }
}