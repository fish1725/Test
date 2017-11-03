using System;
using System.Collections.Generic;
using g3;
using UnityEngine;
using Vox.Database;
using Vox.Handler;
using Vox.Utils;

namespace Vox.Core
{
	public class Chunk : IBlockSet
    {
        private readonly BlockData[] _blocks;
        private readonly Dictionary<ushort, Block> _blockTypes = new Dictionary<ushort, Block>();
        private readonly AxisAlignedBox3i _bounds;
        private readonly ChunkRenderGeometryHandler _renderGeometryHandler;

        public ARenderGeometryHandler RenderGeometryHandler
        {
            get
            {
                return _renderGeometryHandler;
            }
        }

        public Chunk()
        {
            _blocks = new BlockData[Env.ChunkSizePow3];
            _bounds = new AxisAlignedBox3i(0, 0, 0, Env.ChunkSize, Env.ChunkSize, Env.ChunkSize);
            _renderGeometryHandler = new ChunkRenderGeometryHandler(this, null);
        }

        public BlockData GetBlockData(Vector3i pos)
        {
            if (pos.x < 0 || pos.x >= Env.ChunkSize || pos.y < 0 || pos.y >= Env.ChunkSize || pos.z < 0 || pos.z >= Env.ChunkSize)
                return BlockDatabase.AirBlock;
            return _blocks[GetIndex1DFrom3D(pos.x, pos.y, pos.z)];
        }

        public Block GetBlock(Vector3i pos)
        {
            return _blockTypes[GetBlockData(pos).Type];
        }

        public int GetIndex1DFrom3D(int x, int y, int z)
        {
            return Helpers.GetChunkIndex1DFrom3D(x, y, z);
        }

        public BlockData GetBlockData(int x, int y, int z)
        {
            if (x < 0 || x >= Env.ChunkSize || y < 0 || y >= Env.ChunkSize || z < 0 || z >= Env.ChunkSize)
                return BlockDatabase.AirBlock;
            return _blocks[GetIndex1DFrom3D(x, y, z)];
        }

        public Block GetBlock(int x, int y, int z)
        {
            return _blockTypes[GetBlockData(x, y, z).Type];
        }

        public int GetBlockCount()
        {
            return Env.ChunkSizePow3;
        }

        public AxisAlignedBox3i GetBounds()
        {
            return _bounds;
        }

        public void SetBlockData(Vector3i pos, BlockData data)
        {
            try
            {
                _blocks[GetIndex1DFrom3D(pos.x, pos.y, pos.z)] = data;
            }
            catch (Exception e)
            {
                Debug.LogFormat("{0}", pos);
                throw e;
            }
        }

        public void SetBlockData(int x, int y, int z, BlockData data)
        {
            _blocks[GetIndex1DFrom3D(x, y, z)] = data;
        }

        public void SetBlockType(ushort type, Block block)
        {
            _blockTypes[type] = block;
        }

		public float BlockSize {
			get {
				throw new NotImplementedException ();
			}
		}
    }
}