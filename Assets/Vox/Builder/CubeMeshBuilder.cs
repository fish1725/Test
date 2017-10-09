using System;
using UnityEngine;
using Vox.Core;

namespace Vox.Builder
{
    public abstract class CubeMeshBuilder : AMeshBuilder
    {
        protected abstract bool CanBuild(Block block);
        protected abstract bool CanMerge(Block block1, Block block2);
        protected abstract void BuildBox(IBlockSet blockSet, Block block, int minX, int minY, int minZ, int maxX, int maxY, int maxZ);

        private bool ExpandX(IBlockSet blocks, ref bool[] mask, Block block, int y1, int z1, ref int x2, int y2, int z2)
        {

            // Check the quad formed by YZ axes and try to expand the X axis
            for (int y = y1; y < y2; ++y)
            {
                for (int z = z1; z < z2; ++z)
                {
                    if (mask[blocks.GetIndex1DFrom3D(x2, y, z)] || !CanMerge(block, blocks.GetBlock(x2, y, z)))
                        return false;
                }
            }

            // If the box can expand, mark the position as tested and expand the X axis
            for (int y = y1; y < y2; ++y)
            {
                for (int z = z1; z < z2; ++z)
                    mask[blocks.GetIndex1DFrom3D(x2, y, z)] = true;
            }

            ++x2;
            return true;
        }

        private bool ExpandY(IBlockSet blocks, ref bool[] mask, Block block, int x1, int z1, int x2, ref int y2, int z2)
        {

            // Check the quad formed by XZ axes and try to expand the Y axis
            for (int z = z1; z < z2; ++z)
            {
                for (int x = x1; x < x2; ++x)
                {
                    if (mask[blocks.GetIndex1DFrom3D(x, y2, z)] || !CanMerge(block, blocks.GetBlock(x, y2, z)))
                        return false;
                }
            }

            // If the box can expand, mark the position as tested and expand the X axis
            for (int z = z1; z < z2; ++z)
            {
                for (int x = x1; x < x2; ++x)
                    mask[blocks.GetIndex1DFrom3D(x, y2, z)] = true;
            }

            ++y2;
            return true;
        }

        private bool ExpandZ(IBlockSet blocks, ref bool[] mask, Block block, int x1, int y1, int x2, int y2, ref int z2)
        {

            // Check the quad formed by XY axes and try to expand the Z axis
            for (int y = y1; y < y2; ++y)
            {
                for (int x = x1; x < x2; ++x)
                {
                    if (mask[blocks.GetIndex1DFrom3D(x, y, z2)] || !CanMerge(block, blocks.GetBlock(x, y, z2)))
                        return false;
                }
            }

            // If the box can expand, mark the position as tested and expand the X axis
            for (int y = y1; y < y2; ++y)
            {
                for (int x = x1; x < x2; ++x)
                    mask[blocks.GetIndex1DFrom3D(x, y, z2)] = true;
            }

            ++z2;
            return true;
        }

        public override void Build(IBlockSet blockSet)
        {
            var mask = Globals.BoolArrayPool.Pop(blockSet.GetBlockCount());
            var bounds = blockSet.GetBounds();
            Array.Clear(mask, 0, mask.Length);

            for (var x = bounds.Min.x; x < bounds.Max.x; x++)
            {
                for (var y = bounds.Min.y; y < bounds.Max.y; y++)
                {
                    for (var z = bounds.Min.z; z < bounds.Max.z; z++)
                    {
                        if (mask[blockSet.GetIndex1DFrom3D(x, y, z)])
                            continue;

                        mask[blockSet.GetIndex1DFrom3D(x, y, z)] = true;

						var block = blockSet.GetBlock(x, y, z);

                        if (!CanBuild(block))
							continue;
                        

                        int x1 = x, y1 = y, z1 = z, x2 = x + 1, y2 = y + 1, z2 = z + 1;

                        bool expandX = true;
                        bool expandY = true;
                        bool expandZ = true;
                        bool expand;

                        // Try to expand our box in all axes
                        do
                        {
                            expand = false;

                            if (expandY)
                            {
                                expandY = y2 < bounds.Max.y &&
                                          ExpandY(blockSet, ref mask, block, x1, z1, x2, ref y2, z2);
                                expand = expandY;
                            }
                            if (expandZ)
                            {
                                expandZ = z2 < bounds.Max.z &&
                                          ExpandZ(blockSet, ref mask, block, x1, y1, x2, y2, ref z2);
                                expand = expand | expandZ;
                            }
                            if (expandX)
                            {
                                expandX = x2 < bounds.Max.x &&
                                          ExpandX(blockSet, ref mask, block, y1, z1, ref x2, y2, z2);
                                expand = expand | expandX;
                            }
                        } while (expand);

                        BuildBox(blockSet, block, x1, y1, z1, x2, y2, z2);
                    }
                }
            }

			Globals.BoolArrayPool.Push(mask);
		}
    }

}