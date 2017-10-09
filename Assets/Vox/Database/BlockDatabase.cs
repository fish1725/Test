using UnityEngine;
using System.Collections;
using Vox.Core;
using System.Collections.Generic;

namespace Vox.Database
{
    public class BlockDatabase
    {
        public static readonly ushort AirType = 0;
        public static readonly BlockData AirBlock = new BlockData(AirType, false);

        private BlockConfig[] _configs;
        private Dictionary<ushort, Block> _blocks = new Dictionary<ushort, Block>();
            
        public Block GetBlock(ushort type) {
            return _blocks[type];
        }

        public void SetBlock(ushort type, Block block) {
            _blocks[type] = block;
        }
    }
}