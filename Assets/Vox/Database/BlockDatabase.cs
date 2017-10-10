using UnityEngine;
using System.Collections;
using Vox.Core;
using System.Collections.Generic;
using Vox.Core.Blocks;

namespace Vox.Database
{
    public class BlockDatabase
    {
        public static readonly ushort AirType = 0;
        public static readonly BlockData AirBlock = new BlockData(AirType, false);

        private BlockConfig[] _configs;
        private Dictionary<ushort, Block> _blocks = new Dictionary<ushort, Block>();
            
        public Block GetBlock(ushort type) {
			if (!_blocks.ContainsKey (type)) {
				_blocks [type] = new ColoredBlock (new BlockConfig { Type = type, Name = "CB", Solid = true });
			}
            return _blocks[type];
        }

        public void SetBlock(ushort type, Block block) {
            _blocks[type] = block;
        }
    }
}