using UnityEngine;
using System.Collections;
using Vox.Builder.RenderGeometry;
using Vox.Core;
using g3;
using Vox.Common;

namespace Vox.Utils
{
    public class BlockUtils
    {
		/// All faces in the engine are build in the following order:
		///     1--2
		///     |  |
		///     |  |
		///     0--3

		public static readonly Vector3[][] PaddingOffsets =
		{
			new[]
			{
                // up
                new Vector3(-Env.BlockFacePadding, +Env.BlockFacePadding, -Env.BlockFacePadding),
				new Vector3(-Env.BlockFacePadding, +Env.BlockFacePadding, +Env.BlockFacePadding),
				new Vector3(+Env.BlockFacePadding, +Env.BlockFacePadding, +Env.BlockFacePadding),
				new Vector3(+Env.BlockFacePadding, +Env.BlockFacePadding, -Env.BlockFacePadding)
			},
			new[]
			{
                // down
                new Vector3(+Env.BlockFacePadding, -Env.BlockFacePadding, -Env.BlockFacePadding),
				new Vector3(+Env.BlockFacePadding, -Env.BlockFacePadding, +Env.BlockFacePadding),
				new Vector3(-Env.BlockFacePadding, -Env.BlockFacePadding, +Env.BlockFacePadding),
				new Vector3(-Env.BlockFacePadding, -Env.BlockFacePadding, -Env.BlockFacePadding),
			},

			new[]
			{
                // forward
                new Vector3(+Env.BlockFacePadding, -Env.BlockFacePadding, +Env.BlockFacePadding),
				new Vector3(+Env.BlockFacePadding, +Env.BlockFacePadding, +Env.BlockFacePadding),
				new Vector3(-Env.BlockFacePadding, +Env.BlockFacePadding, +Env.BlockFacePadding),
				new Vector3(-Env.BlockFacePadding, -Env.BlockFacePadding, +Env.BlockFacePadding)
			},
			new[]
			{
                // back
                new Vector3(-Env.BlockFacePadding, -Env.BlockFacePadding, -Env.BlockFacePadding),
				new Vector3(-Env.BlockFacePadding, +Env.BlockFacePadding, -Env.BlockFacePadding),
				new Vector3(+Env.BlockFacePadding, +Env.BlockFacePadding, -Env.BlockFacePadding),
				new Vector3(+Env.BlockFacePadding, -Env.BlockFacePadding, -Env.BlockFacePadding),
			},

			new[]
			{
                // right
                new Vector3(+Env.BlockFacePadding, -Env.BlockFacePadding, -Env.BlockFacePadding),
				new Vector3(+Env.BlockFacePadding, +Env.BlockFacePadding, -Env.BlockFacePadding),
				new Vector3(+Env.BlockFacePadding, +Env.BlockFacePadding, +Env.BlockFacePadding),
				new Vector3(+Env.BlockFacePadding, -Env.BlockFacePadding, +Env.BlockFacePadding)
			},
			new[]
			{
                // left
                new Vector3(-Env.BlockFacePadding, -Env.BlockFacePadding, +Env.BlockFacePadding),
				new Vector3(-Env.BlockFacePadding, +Env.BlockFacePadding, +Env.BlockFacePadding),
				new Vector3(-Env.BlockFacePadding, +Env.BlockFacePadding, -Env.BlockFacePadding),
				new Vector3(-Env.BlockFacePadding, -Env.BlockFacePadding, -Env.BlockFacePadding),
			}
		};

		public static BlockFace BuildBlockFace(IBlockSet blocks, Block block, int x, int y, int z, Vector3i direction)
		{
			var bf = new BlockFace();
			bf.block = block;
			bf.direction = direction;
			bf.aoData = BlockUtils.CalcBlockFaceAO(blocks, x, y, z, direction);
			return bf;
		}

        public static BlockFaceAOData CalcBlockFaceAO(IBlockSet blocks, int x, int y, int z, Vector3i direction) {
			// Side blocks
			bool nSolid, eSolid, sSolid, wSolid;
			// Corner blocks
			bool nwSolid, neSolid, seSolid, swSolid;

            if (direction == Vector3i.up) {
				swSolid = blocks.GetBlock(x - 1, y + 1, z - 1).Solid; // -1,1,-1
				sSolid = blocks.GetBlock(x, y + 1, z - 1).Solid;      //  0,1,-1
				seSolid = blocks.GetBlock(x + 1, y + 1, z - 1).Solid; //  1,1,-1
				wSolid = blocks.GetBlock(x - 1, y + 1, z).Solid;   // -1,1, 0
				eSolid = blocks.GetBlock(x + 1, y + 1, z).Solid;   //  1,1, 0
				nwSolid = blocks.GetBlock(x - 1, y + 1, z + 1).Solid; // -1,1, 1
				nSolid = blocks.GetBlock(x, y + 1, z + 1).Solid;      //  0,1, 1
				neSolid = blocks.GetBlock(x + 1, y + 1, z + 1).Solid; //  1,1, 1
            } else if (direction == Vector3i.down) {
				swSolid = blocks.GetBlock(x + 1, y - 1, z - 1).Solid; // -1,-1,-1
				sSolid = blocks.GetBlock(x, y - 1, z - 1).Solid;      //  0,-1,-1
				seSolid = blocks.GetBlock(x - 1, y - 1, z - 1).Solid; //  1,-1,-1
				wSolid = blocks.GetBlock(x + 1, y - 1, z).Solid;   // -1,-1, 0
				eSolid = blocks.GetBlock(x - 1, y - 1, z).Solid;   //  1,-1, 0
				nwSolid = blocks.GetBlock(x + 1,y - 1, z + 1).Solid; // -1,-1, 1
				nSolid = blocks.GetBlock(x, y - 1, z + 1).Solid;      //  0,-1, 1
				neSolid = blocks.GetBlock(x - 1, y - 1, z + 1).Solid; //  1,-1, 1
            } else if (direction == Vector3i.forward) {
				swSolid = blocks.GetBlock(x + 1, y - 1, z + 1).Solid; // -1,-1,1
				seSolid = blocks.GetBlock(x - 1, y - 1, z + 1).Solid; //  1,-1,1
				wSolid = blocks.GetBlock(x + 1, y, z + 1).Solid;   // -1, 0,1
				eSolid = blocks.GetBlock(x - 1, y, z + 1).Solid;   //  1, 0,1
				nwSolid = blocks.GetBlock(x + 1, y + 1, z + 1).Solid; // -1, 1,1
				sSolid = blocks.GetBlock(x, y - 1, z + 1).Solid;      //  0,-1,1
				nSolid = blocks.GetBlock(x, y + 1, z + 1).Solid;      //  0, 1,1
				neSolid = blocks.GetBlock(x - 1, y + 1, z + 1).Solid; //  1, 1,1
            } else if (direction == Vector3i.back) {
				swSolid = blocks.GetBlock(x - 1, y - 1, z - 1).Solid; // -1,-1,-1
				seSolid = blocks.GetBlock(x + 1, y - 1, z - 1).Solid; //  1,-1,-1
				wSolid = blocks.GetBlock(x - 1, y, z - 1).Solid;   // -1, 0,-1
				eSolid = blocks.GetBlock(x + 1, y, z - 1).Solid;   //  1, 0,-1
				nwSolid = blocks.GetBlock(x - 1, y + 1, z - 1).Solid; // -1, 1,-1
				sSolid = blocks.GetBlock(x, y - 1, z - 1).Solid;      //  0,-1,-1
				nSolid = blocks.GetBlock(x, y + 1, z - 1).Solid;      //  0, 1,-1
				neSolid = blocks.GetBlock(x + 1, y + 1, z - 1).Solid; //  1, 1,-1
            } else if (direction == Vector3i.right) {
				swSolid = blocks.GetBlock(x + 1, y - 1, z - 1).Solid;   // 1,-1,-1
				sSolid = blocks.GetBlock(x + 1, y - 1, z).Solid;                      // 1,-1, 0
				seSolid = blocks.GetBlock(x + 1, y - 1, z + 1).Solid;   // 1,-1, 1
				wSolid = blocks.GetBlock(x + 1, y, z - 1).Solid;     // 1, 0,-1
				eSolid = blocks.GetBlock(x + 1, y, z + 1).Solid;     // 1, 0, 1
				nwSolid = blocks.GetBlock(x + 1, y + 1, z - 1).Solid;   // 1, 1,-1
				nSolid = blocks.GetBlock(x + 1, y + 1, z).Solid;                      // 1, 1, 0
				neSolid = blocks.GetBlock(x + 1, y + 1, z + 1).Solid;   // 1, 1, 1
            } else {
				swSolid = blocks.GetBlock(x - 1, y - 1, z + 1).Solid;  // -1,-1,-1
				sSolid = blocks.GetBlock(x - 1, y - 1, z).Solid;                     // -1,-1, 0
				seSolid = blocks.GetBlock(x - 1, y - 1, z - 1).Solid;  // -1,-1, 1
				wSolid = blocks.GetBlock(x - 1, y, z + 1).Solid;    // -1, 0,-1
				eSolid = blocks.GetBlock(x - 1, y, z - 1).Solid;    // -1, 0, 1
				nwSolid = blocks.GetBlock(x - 1, y + 1, z + 1).Solid;  // -1, 1,-1
				nSolid = blocks.GetBlock(x - 1, y + 1, z).Solid;                     // -1, 1, 0
				neSolid = blocks.GetBlock(x - 1, y + 1, z - 1).Solid;  // -1, 1, 1
			}


            return new BlockFaceAOData(nwSolid, nSolid, neSolid, eSolid, seSolid, sSolid, swSolid, wSolid);
		}

		public static void AdjustColors(VertexData[] vertexData, BlockFaceAOData light)
		{
			AdjustColorsAO(vertexData, light, 0.5f);
		}

		private static void AdjustColorsAO(VertexData[] vertexData, BlockFaceAOData light, float strength)
		{
			// 0.33f for there are 3 degrees of AO darkening (0.33f * 3 =~ 1f)
			float ne = 1f - light.neAO * 0.33f * strength;
			float se = 1f - light.seAO * 0.33f * strength;
			float sw = 1f - light.swAO * 0.33f * strength;
			float nw = 1f - light.nwAO * 0.33f * strength;

			AdjustColors(vertexData, sw, nw, ne, se, light.FaceRotationNecessary);
		}

		public static void AdjustColors(VertexData[] data, float sw, float nw, float ne, float se, bool rotated)
		{
            sw = Mathf.Clamp(sw, 0f, 1f);
            nw = Mathf.Clamp(nw, 0f, 1f);
            ne = Mathf.Clamp(ne, 0f, 1f);
            se = Mathf.Clamp(se, 0f, 1f);

			if (!rotated)
			{
				data[0].Color = ToColor32(ref data[0].Color, sw);
				data[1].Color = ToColor32(ref data[1].Color, nw);
				data[2].Color = ToColor32(ref data[2].Color, ne);
				data[3].Color = ToColor32(ref data[3].Color, se);
			}
			else
			{
				data[0].Color = ToColor32(ref data[0].Color, nw);
				data[1].Color = ToColor32(ref data[1].Color, ne);
				data[2].Color = ToColor32(ref data[2].Color, se);
				data[3].Color = ToColor32(ref data[3].Color, sw);
			}
		}

		private static Color32 ToColor32(ref Color32 col, float coef)
		{
			return new Color32(
				(byte)(col.r * coef),
				(byte)(col.g * coef),
				(byte)(col.b * coef),
				col.a
				);
		}
    }
}