using UnityEngine;
using System.Collections;
using Vox.Common;
using Vox.Utils;

namespace Vox.Core.Blocks
{
    public class ColoredBlock : Block
    {
        private static readonly Color32 WHITE = new Color32(255, 255, 255, 255);
        public ColoredBlock(BlockConfig config) : base(config)
        {
            
        }

		public Color32 GetColor(BlockData block) {
			var r = (byte)((block.Type >> 10 & 0x1F) << 3 | ((block.Type >> 10) & 0x7));
			var g = (byte)((block.Type >> 5 & 0x1F) << 3 | ((block.Type >> 5) & 0x7));
			var b = (byte)((block.Type & 0x1F) << 3 | (block.Type & 0x7));
			return new Color32 (r, g, b, 255);
		}

        public override void BuildFace(IBlockSet blocks, ref Vector3[] face, Builder.RenderGeometry.BlockFace blockFace)
        {
            //Debug.LogFormat("face: ({0}) ({1}) ({2}) ({3})", face[0], face[1], face[2], face[3]);
            //var backface = (blockFace.direction.x + blockFace.direction.y) < 0 || blockFace.direction.z > 0;
            VertexData[] vertexData = Globals.VertexDataArrayPool.PopExact(4);
			var color = GetColor (blocks.GetBlockData(blockFace.position));
			vertexData[0].Vertex = face[0];
			vertexData[0].Color = color;
			vertexData[0].UV = Vector2.zero;

			vertexData[1].Vertex = face[1];
			vertexData[1].Color = color;
			vertexData[1].UV = Vector2.zero;

			vertexData[2].Vertex = face[2];
			vertexData[2].Color = color;
			vertexData[2].UV = Vector2.zero;

			vertexData[3].Vertex = face[3];
			vertexData[3].Color = color;
			vertexData[3].UV = Vector2.zero;

            BlockUtils.AdjustColors(vertexData, blockFace.aoData);

            var batcher = blocks.RenderGeometryHandler.Batcher;
            batcher.AddFace(vertexData, blockFace.materialId);
			Globals.VertexDataArrayPool.Push(vertexData);
        }
    }
}