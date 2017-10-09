using UnityEngine;
using System.Collections;
using System;
using Vox.Core;
using Vox.Database;

namespace Vox.Builder.RenderGeometry
{
    public class CubeRenderGeometryBuilder : CubeMeshBuilder
    {
        protected override void BuildBox(IBlockSet blockSet, Block block, int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
        {
            var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.transform.position = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, (minZ + maxZ) / 2f);
            box.transform.localScale = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
            box.transform.name = string.Format("{0},{1},{2} {3},{4},{5}", minX, minY, minZ, maxX, maxY, maxZ);
        }

        protected override bool CanBuild(Block block)
        {
            return block.Type != BlockDatabase.AirType;
        }

        protected override bool CanMerge(Block block1, Block block2)
        {
            return block1.Type == block2.Type;
        }
    }
}