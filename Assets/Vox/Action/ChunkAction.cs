using UnityEngine;
using System.Collections;
using Vox.Core;

namespace Vox.Action
{
    public abstract class ChunkAction
    {
        private Chunk _chunk;

        public ChunkAction(Chunk chunk) {
            _chunk = chunk;
        }

        public virtual void Execute() {
            throw new System.NotImplementedException();
        }
    }
}