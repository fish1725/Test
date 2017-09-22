using UnityEngine;
using System.Collections;
using Vox.Core;

namespace Vox.Database
{
    public class BlockDatabase
    {
        public static readonly ushort AirType = 0;
        public static readonly BlockData AirBlock = new BlockData(AirType, false);

        private BlockConfig[] _configs;
    }
}