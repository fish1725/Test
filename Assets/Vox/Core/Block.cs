using UnityEngine;
using System.Collections;
using Vox.Builder.Geometry;
using g3;

namespace Vox.Core
{
    public class Block
    {
        private readonly BlockConfig _config;

        public Block() {
            _config = null;
        }

        public Block(BlockConfig config)
		{
			_config = config;
		}

        public string Name { get { return _config.Name; }}
        public ushort Type { get { return _config.Type; }}
        public int RenderMaterialID { get { return _config.RenderMaterialID; }}
        public int PhysicMaterialID { get { return _config.PhysicMaterialID; }}
        public bool Solid { get { return _config.Solid; }}

        public virtual void BuildFace(IBlockSet blocks, ref Vector3i[] face, BlockFace blockFace) {
            
        }
    }
}