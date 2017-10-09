using UnityEngine;
using System;
using Vox.Core;
using Vox.Database;
using g3;
using Vox.Utils;

namespace Vox.Builder.RenderGeometry
{
    public class CubeMeshRenderGeometryBuilder : CubeMeshBuilder
    {
        private bool ExpandX(IBlockSet blocks, ref BlockFace[] faces, ref bool[] mask, BlockFace blockFace, int y1, int z1, ref int x2, int y2, int z2)
        {

            // Check the quad formed by YZ axes and try to expand the X axis
            for (int y = y1; y < y2; ++y)
            {
                for (int z = z1; z < z2; ++z)
                {
                    if (mask[blocks.GetIndex1DFrom3D(x2, y, z)] || !CanMergeFace(blockFace, faces[blocks.GetIndex1DFrom3D(x2, y, z)]))
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

        private bool ExpandY(IBlockSet blocks, ref BlockFace[] faces, ref bool[] mask, BlockFace blockFace, int x1, int z1, int x2, ref int y2, int z2)
        {

            // Check the quad formed by XZ axes and try to expand the Y axis
            for (int z = z1; z < z2; ++z)
            {
                for (int x = x1; x < x2; ++x)
                {
                    if (mask[blocks.GetIndex1DFrom3D(x, y2, z)] || !CanMergeFace(blockFace, faces[blocks.GetIndex1DFrom3D(x, y2, z)]))
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

        private bool ExpandZ(IBlockSet blocks, ref BlockFace[] faces, ref bool[] mask, BlockFace blockFace, int x1, int y1, int x2, int y2, ref int z2)
        {

            for (int y = y1; y < y2; ++y)
            {
                for (int x = x1; x < x2; ++x)
                {
                    if (mask[blocks.GetIndex1DFrom3D(x, y, z2)] || !CanMergeFace(blockFace, faces[blocks.GetIndex1DFrom3D(x, y, z2)]))
                        return false;
                }
            }

            for (int y = y1; y < y2; ++y)
            {
                for (int x = x1; x < x2; ++x)
                {
                    //Debug.LogFormat("mask set true {0}, {1}, {2}", x, y, z2);
                    mask[blocks.GetIndex1DFrom3D(x, y, z2)] = true;
                }
            }
            //Debug.LogFormat("ExpandZ {0}, {1}, {2}, {3}, {4}", x1, y1, x2, y2, z2);
            ++z2;
            return true;
        }



        protected override void BuildBox(IBlockSet blockSet, Block block, int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
        {
            var faces = Globals.blockFacePool.Pop(blockSet.GetBlockCount());
            var mask = Globals.BoolArrayPool.Pop(blockSet.GetBlockCount());
            var face = Globals.Vector3ArrayPool.Pop(4);
            int x, y, z;


            Array.Clear(faces, 0, faces.Length);
            Array.Clear(mask, 0, mask.Length);
            // left
            x = minX;
            for (y = minY; y < maxY; y++)
            {
                for (z = minZ; z < maxZ; z++)
                {
                    if (!CanBuildFace(blockSet, x, y, z, Vector3i.left))
                        continue;

                    var curBlock = blockSet.GetBlock(x, y, z);
                    faces[blockSet.GetIndex1DFrom3D(x, y, z)] = BlockUtils.BuildBlockFace(blockSet, curBlock, x, y, z, Vector3i.left);


                }
            }
            x = minX;
            for (y = minY; y < maxY; y++)
            {
                for (z = minZ; z < maxZ; z++)
                {
                    var index = blockSet.GetIndex1DFrom3D(x, y, z);

                    //Debug.LogFormat("mask get {3} {0}, {1}, {2}", x, y, z, mask[index]);
                    if (mask[index])
                        continue;
                    //Debug.LogFormat("mask set true {0}, {1}, {2}", x, y, z);
                    mask[index] = true;
                    //Debug.LogFormat("faces get {3} {0}, {1}, {2}", x, y, z, faces[index].block);
                    if (faces[index].block == null)
                        continue;

                    var blockFace = faces[index];
                    int y1 = y, z1 = z, y2 = y + 1, z2 = z + 1;

                    bool expandY = true;
                    bool expandZ = true;
                    bool expand;

                    do
                    {
                        expand = false;

                        if (expandY)
                        {
                            expandY = y2 < maxY &&
                                      ExpandY(blockSet, ref faces, ref mask, blockFace, x, z1, x + 1, ref y2, z2);
                            expand = expandY;
                        }
                        if (expandZ)
                        {
                            expandZ = z2 < maxZ &&
                                      ExpandZ(blockSet, ref faces, ref mask, blockFace, x, y1, x + 1, y2, ref z2);
                            expand = expand | expandZ;
                        }
                    } while (expand);

                    bool rotated = blockFace.aoData.FaceRotationNecessary;
                    if (!rotated)
                    {
                        face[0] = new Vector3(x, y1, z2) + BlockUtils.PaddingOffsets[5][0];
                        face[1] = new Vector3(x, y2, z2) + BlockUtils.PaddingOffsets[5][1];
                        face[2] = new Vector3(x, y2, z1) + BlockUtils.PaddingOffsets[5][2];
                        face[3] = new Vector3(x, y1, z1) + BlockUtils.PaddingOffsets[5][3];
                    }
                    else
                    {
                        face[0] = new Vector3(x, y2, z2) + BlockUtils.PaddingOffsets[5][1];
                        face[1] = new Vector3(x, y2, z1) + BlockUtils.PaddingOffsets[5][2];
                        face[2] = new Vector3(x, y1, z1) + BlockUtils.PaddingOffsets[5][3];
                        face[3] = new Vector3(x, y1, z2) + BlockUtils.PaddingOffsets[5][0];
                    }

                    block.BuildFace(blockSet, ref face, blockFace);
                }
            }

            Array.Clear(faces, 0, faces.Length);
            Array.Clear(mask, 0, mask.Length);
            // right
            x = maxX - 1;
            for (y = minY; y < maxY; y++)
            {
                for (z = minZ; z < maxZ; z++)
                {
                    if (!CanBuildFace(blockSet, x, y, z, Vector3i.right))
                        continue;

                    var curBlock = blockSet.GetBlock(x, y, z);
                    faces[blockSet.GetIndex1DFrom3D(x, y, z)] = BlockUtils.BuildBlockFace(blockSet, curBlock, x, y, z, Vector3i.right);


                }
            }
            x = maxX - 1;
            for (y = minY; y < maxY; y++)
            {
                for (z = minZ; z < maxZ; z++)
                {
                    var index = blockSet.GetIndex1DFrom3D(x, y, z);
                    if (mask[index])
                        continue;
                    mask[index] = true;

                    if (faces[index].block == null)
                        continue;

                    var blockFace = faces[index];
                    int y1 = y, z1 = z, y2 = y + 1, z2 = z + 1;

                    bool expandY = true;
                    bool expandZ = true;
                    bool expand;

                    do
                    {
                        expand = false;

                        if (expandY)
                        {
                            expandY = y2 < maxY &&
                                      ExpandY(blockSet, ref faces, ref mask, blockFace, x, z1, x + 1, ref y2, z2);
                            expand = expandY;
                        }
                        if (expandZ)
                        {
                            expandZ = z2 < maxZ &&
                                      ExpandZ(blockSet, ref faces, ref mask, blockFace, x, y1, x + 1, y2, ref z2);
                            expand = expand | expandZ;
                        }
                    } while (expand);

                    bool rotated = blockFace.aoData.FaceRotationNecessary;
                    if (!rotated)
                    {
                        face[0] = new Vector3(x + 1, y1, z1) + BlockUtils.PaddingOffsets[4][0];
                        face[1] = new Vector3(x + 1, y2, z1) + BlockUtils.PaddingOffsets[4][1];
                        face[2] = new Vector3(x + 1, y2, z2) + BlockUtils.PaddingOffsets[4][2];
                        face[3] = new Vector3(x + 1, y1, z2) + BlockUtils.PaddingOffsets[4][3];
                    }
                    else
                    {
                        face[0] = new Vector3(x + 1, y2, z1) + BlockUtils.PaddingOffsets[4][1];
                        face[1] = new Vector3(x + 1, y2, z2) + BlockUtils.PaddingOffsets[4][2];
                        face[2] = new Vector3(x + 1, y1, z2) + BlockUtils.PaddingOffsets[4][3];
                        face[3] = new Vector3(x + 1, y1, z1) + BlockUtils.PaddingOffsets[4][0];
                    }

                    block.BuildFace(blockSet, ref face, blockFace);
                }
            }

            Array.Clear(faces, 0, faces.Length);
            Array.Clear(mask, 0, mask.Length);
            // top
            y = maxY - 1;
            for (x = minX; x < maxX; x++)
            {
                for (z = minZ; z < maxZ; z++)
                {
                    if (!CanBuildFace(blockSet, x, y, z, Vector3i.up))
                        continue;

                    var curBlock = blockSet.GetBlock(x, y, z);
                    faces[blockSet.GetIndex1DFrom3D(x, y, z)] = BlockUtils.BuildBlockFace(blockSet, curBlock, x, y, z, Vector3i.up);


                }
            }
            y = maxY - 1;
            for (x = minX; x < maxX; x++)
            {
                for (z = minZ; z < maxZ; z++)
                {
                    var index = blockSet.GetIndex1DFrom3D(x, y, z);
                    if (mask[index])
                        continue;
                    mask[index] = true;

                    if (faces[index].block == null)
                        continue;

                    var blockFace = faces[index];
                    int x1 = x, z1 = z, x2 = x + 1, z2 = z + 1;

                    bool expandX = true;
                    bool expandZ = true;
                    bool expand;

                    do
                    {
                        expand = false;

                        if (expandX)
                        {
                            expandX = x2 < maxX &&
                                      ExpandX(blockSet, ref faces, ref mask, blockFace, y, z1, ref x2, y + 1, z2);
                            expand = expandX;
                        }
                        if (expandZ)
                        {
                            expandZ = z2 < maxZ &&
                                      ExpandZ(blockSet, ref faces, ref mask, blockFace, x1, y, x2, y + 1, ref z2);
                            expand = expand | expandZ;
                        }
                    } while (expand);

                    bool rotated = blockFace.aoData.FaceRotationNecessary;
                    if (!rotated)
                    {
                        face[0] = new Vector3(x1, y + 1, z1) + BlockUtils.PaddingOffsets[0][0];
                        face[1] = new Vector3(x1, y + 1, z2) + BlockUtils.PaddingOffsets[0][1];
                        face[2] = new Vector3(x2, y + 1, z2) + BlockUtils.PaddingOffsets[0][2];
                        face[3] = new Vector3(x2, y + 1, z1) + BlockUtils.PaddingOffsets[0][3];
                    }
                    else
                    {
                        face[0] = new Vector3(x1, y + 1, z2) + BlockUtils.PaddingOffsets[0][1];
                        face[1] = new Vector3(x2, y + 1, z2) + BlockUtils.PaddingOffsets[0][2];
                        face[2] = new Vector3(x2, y + 1, z1) + BlockUtils.PaddingOffsets[0][3];
                        face[3] = new Vector3(x1, y + 1, z1) + BlockUtils.PaddingOffsets[0][0];
                    }

                    block.BuildFace(blockSet, ref face, blockFace);
                }
            }

            Array.Clear(faces, 0, faces.Length);
            Array.Clear(mask, 0, mask.Length);
            // bottom
            y = minY;
            for (x = minX; x < maxX; x++)
            {
                for (z = minZ; z < maxZ; z++)
                {
                    if (!CanBuildFace(blockSet, x, y, z, Vector3i.down))
                        continue;

                    var curBlock = blockSet.GetBlock(x, y, z);
                    faces[blockSet.GetIndex1DFrom3D(x, y, z)] = BlockUtils.BuildBlockFace(blockSet, curBlock, x, y, z, Vector3i.down);


                }
            }
            y = minY;
            for (x = minX; x < maxX; x++)
            {
                for (z = minZ; z < maxZ; z++)
                {
                    var index = blockSet.GetIndex1DFrom3D(x, y, z);
                    if (mask[index])
                        continue;
                    mask[index] = true;

                    if (faces[index].block == null)
                        continue;

                    var blockFace = faces[index];
                    int x1 = x, z1 = z, x2 = x + 1, z2 = z + 1;

                    bool expandX = true;
                    bool expandZ = true;
                    bool expand;

                    do
                    {
                        expand = false;

                        if (expandX)
                        {
                            expandX = x2 < maxX &&
                                      ExpandX(blockSet, ref faces, ref mask, blockFace, y, z1, ref x2, y + 1, z2);
                            expand = expandX;
                        }
                        if (expandZ)
                        {
                            expandZ = z2 < maxZ &&
                                      ExpandZ(blockSet, ref faces, ref mask, blockFace, x1, y, x2, y + 1, ref z2);
                            expand = expand | expandZ;
                        }
                    } while (expand);

                    bool rotated = blockFace.aoData.FaceRotationNecessary;
                    if (!rotated)
                    {
                        face[0] = new Vector3(x2, y, z1) + BlockUtils.PaddingOffsets[1][0];
                        face[1] = new Vector3(x2, y, z2) + BlockUtils.PaddingOffsets[1][1];
                        face[2] = new Vector3(x1, y, z2) + BlockUtils.PaddingOffsets[1][2];
                        face[3] = new Vector3(x1, y, z1) + BlockUtils.PaddingOffsets[1][3];
                    }
                    else
                    {
                        face[0] = new Vector3(x2, y, z2) + BlockUtils.PaddingOffsets[1][1];
                        face[1] = new Vector3(x1, y, z2) + BlockUtils.PaddingOffsets[1][2];
                        face[2] = new Vector3(x1, y, z1) + BlockUtils.PaddingOffsets[1][3];
                        face[3] = new Vector3(x2, y, z1) + BlockUtils.PaddingOffsets[1][0];
                    }

                    block.BuildFace(blockSet, ref face, blockFace);
                }
            }

            Array.Clear(faces, 0, faces.Length);
            Array.Clear(mask, 0, mask.Length);
            // front
            z = maxZ - 1;
            for (x = minX; x < maxX; x++)
            {
                for (y = minY; y < maxY; y++)
                {
                    if (!CanBuildFace(blockSet, x, y, z, Vector3i.forward))
                        continue;

                    var curBlock = blockSet.GetBlock(x, y, z);
                    faces[blockSet.GetIndex1DFrom3D(x, y, z)] = BlockUtils.BuildBlockFace(blockSet, curBlock, x, y, z, Vector3i.forward);


                }
            }
            z = maxZ - 1;
            for (x = minX; x < maxX; x++)
            {
                for (y = minY; y < maxY; y++)
                {
                    var index = blockSet.GetIndex1DFrom3D(x, y, z);
                    if (mask[index])
                        continue;
                    mask[index] = true;

                    if (faces[index].block == null)
                        continue;

                    var blockFace = faces[index];
                    int x1 = x, y1 = y, x2 = x + 1, y2 = y + 1;

                    bool expandX = true;
                    bool expandY = true;
                    bool expand;

                    do
                    {
                        expand = false;

                        if (expandX)
                        {
                            expandX = x2 < maxX &&
                                      ExpandX(blockSet, ref faces, ref mask, blockFace, y1, z, ref x2, y2, z + 1);
                            expand = expandX;
                        }
                        if (expandY)
                        {
                            expandY = y2 < maxY &&
                                      ExpandY(blockSet, ref faces, ref mask, blockFace, x1, z, x2, ref y2, z + 1);
                            expand = expand | expandY;
                        }
                    } while (expand);

                    bool rotated = blockFace.aoData.FaceRotationNecessary;
                    if (!rotated)
                    {
                        face[0] = new Vector3(x2, y1, z + 1) + BlockUtils.PaddingOffsets[2][0];
                        face[1] = new Vector3(x2, y2, z + 1) + BlockUtils.PaddingOffsets[2][1];
                        face[2] = new Vector3(x1, y2, z + 1) + BlockUtils.PaddingOffsets[2][2];
                        face[3] = new Vector3(x1, y1, z + 1) + BlockUtils.PaddingOffsets[2][3];
                    }
                    else
                    {
                        face[0] = new Vector3(x2, y2, z + 1) + BlockUtils.PaddingOffsets[2][1];
                        face[1] = new Vector3(x1, y2, z + 1) + BlockUtils.PaddingOffsets[2][2];
                        face[2] = new Vector3(x1, y1, z + 1) + BlockUtils.PaddingOffsets[2][3];
                        face[3] = new Vector3(x2, y1, z + 1) + BlockUtils.PaddingOffsets[2][0];
                    }

                    block.BuildFace(blockSet, ref face, blockFace);
                }
            }

            Array.Clear(faces, 0, faces.Length);
            Array.Clear(mask, 0, mask.Length);
            // back
            z = minZ;
            for (x = minX; x < maxX; x++)
            {
                for (y = minY; y < maxY; y++)
                {
                    if (!CanBuildFace(blockSet, x, y, z, Vector3i.back))
                        continue;

                    var curBlock = blockSet.GetBlock(x, y, z);
                    faces[blockSet.GetIndex1DFrom3D(x, y, z)] = BlockUtils.BuildBlockFace(blockSet, curBlock, x, y, z, Vector3i.back);


                }
            }
            z = minZ;
            for (x = minX; x < maxX; x++)
            {
                for (y = minY; y < maxY; y++)
                {
                    var index = blockSet.GetIndex1DFrom3D(x, y, z);
                    if (mask[index])
                        continue;
                    mask[index] = true;

                    if (faces[index].block == null)
                        continue;

                    var blockFace = faces[index];
                    int x1 = x, y1 = y, x2 = x + 1, y2 = y + 1;

                    bool expandX = true;
                    bool expandY = true;
                    bool expand;

                    do
                    {
                        expand = false;

                        if (expandX)
                        {
                            expandX = x2 < maxX &&
                                      ExpandX(blockSet, ref faces, ref mask, blockFace, y1, z, ref x2, y2, z + 1);
                            expand = expandX;
                        }
                        if (expandY)
                        {
                            expandY = y2 < maxY &&
                                      ExpandY(blockSet, ref faces, ref mask, blockFace, x1, z, x2, ref y2, z + 1);
                            expand = expand | expandY;
                        }
                    } while (expand);

                    bool rotated = blockFace.aoData.FaceRotationNecessary;
                    if (!rotated)
                    {
                        face[0] = new Vector3(x1, y1, z) + BlockUtils.PaddingOffsets[3][0];
                        face[1] = new Vector3(x1, y2, z) + BlockUtils.PaddingOffsets[3][1];
                        face[2] = new Vector3(x2, y2, z) + BlockUtils.PaddingOffsets[3][2];
                        face[3] = new Vector3(x2, y1, z) + BlockUtils.PaddingOffsets[3][3];
                    }
                    else
                    {
                        face[0] = new Vector3(x1, y2, z) + BlockUtils.PaddingOffsets[3][1];
                        face[1] = new Vector3(x2, y2, z) + BlockUtils.PaddingOffsets[3][2];
                        face[2] = new Vector3(x2, y1, z) + BlockUtils.PaddingOffsets[3][3];
                        face[3] = new Vector3(x1, y1, z) + BlockUtils.PaddingOffsets[3][0];
                    }

                    block.BuildFace(blockSet, ref face, blockFace);
                }
            }

            Globals.blockFacePool.Push(faces);
            Globals.BoolArrayPool.Push(mask);
            Globals.Vector3ArrayPool.Push(face);
        }


        protected override bool CanBuild(Block block)
        {
            return block.Type != BlockDatabase.AirType;
        }

        protected override bool CanMerge(Block block1, Block block2)
        {
            return block1.Type == block2.Type;
        }

        private bool CanMergeFace(BlockFace blockFace1, BlockFace blockFace2)
        {
            return blockFace1.block != null
                             && blockFace2.block != null
                             && blockFace1.block.Type == blockFace2.block.Type
                             && blockFace1.aoData.Equals(blockFace2.aoData);
        }

        private bool CanBuildFace(IBlockSet blockSet, int x, int y, int z, Vector3i direction)
        {
            if (blockSet.GetBlock(x + direction.x, y + direction.y, z + direction.z).Solid)
                return false;
            return true;
        }
    }
}