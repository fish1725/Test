using UnityEngine;
using System.Collections;
using g3;
using System;
using Vox.Handler;
using Vox.Database;

namespace Vox.Core
{
    public class VoxObject : IBlockSet
    {
		private float _blockSize;

        private int _sizeX;
        private int _sizeY;
        private int _sizeZ;

        private int _minX, _maxX;
        private int _minY, _maxY;
        private int _minZ, _maxZ;

        private BlockData[] _blocks;
        private AxisAlignedBox3i _bounds;
        private ARenderGeometryHandler _renderGeometryHandler;

		public VoxObject(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, float blockSize) {
            _minX = minX;
            _maxX = maxX;
            _minY = minY;
            _maxY = maxY;
            _minZ = minZ;
            _maxZ = maxZ;
            _sizeX = maxX - minX;
            _sizeY = maxY - minY;
            _sizeZ = maxZ - minZ;

			_blockSize = blockSize;

            _blocks = new BlockData[_sizeX * _sizeY * _sizeZ];
            _bounds = new AxisAlignedBox3i(minX, minY, minZ, maxX, maxY, maxZ);
            _renderGeometryHandler = new ChunkRenderGeometryHandler(this, null);
        }

        public ARenderGeometryHandler RenderGeometryHandler {
            get {
                return _renderGeometryHandler;
            }
        }
        public Block GetBlock(Vector3i pos)
        {
            return Globals.blockDatabase.GetBlock(GetBlockData(pos).Type);
        }

        public Block GetBlock(int x, int y, int z)
        {
            try
            {
                //Debug.LogFormat("GetBlock {0}", GetBlockData(x, y, z).Type);
                return Globals.blockDatabase.GetBlock(GetBlockData(x, y, z).Type);
            }
            catch (Exception e)
            {
                Debug.LogFormat("index: {0}, {1} {2} {3}", GetIndex1DFrom3D(x, y, z), x, y, z);
                throw e;
            }

        }

        public int GetBlockCount()
        {
            return _sizeX * _sizeY * _sizeZ;
        }

        public BlockData GetBlockData(Vector3i pos)
        {
            if (pos.x < _minX || pos.x >= _maxX || pos.y < _minY || pos.y >= _maxY || pos.z < _minZ || pos.z >= _maxZ)
                return BlockDatabase.AirBlock;
            return _blocks[GetIndex1DFrom3D(pos.x, pos.y, pos.z)];
        }

        public BlockData GetBlockData(int x, int y, int z)
        {
            if (x < _minX || x >= _maxX || y < _minY || y >= _maxY || z < _minZ || z >= _maxZ)
                return BlockDatabase.AirBlock;
            return _blocks[GetIndex1DFrom3D(x, y, z)];
        }

        public AxisAlignedBox3i GetBounds()
        {
            return _bounds;
        }

        public int GetIndex1DFrom3D(int x, int y, int z)
        {
            int xx = x + Env.ChunkPadding;
            int yy = y + Env.ChunkPadding;
            int zz = z + Env.ChunkPadding;
            return xx - _minX + ((zz - _minZ) * _sizeX) + ((yy - _minY) * _sizeX * _sizeZ);
        }

        public void SetBlockData(Vector3i pos, BlockData data)
        {
            try
            {
                //Debug.LogFormat("index: {0}, {1} {2} {3} {4}", GetIndex1DFrom3D(pos.x, pos.y, pos.z), pos.x, pos.y, pos.z, data);
                _blocks[GetIndex1DFrom3D(pos.x, pos.y, pos.z)] = data;
            } catch (Exception e) {
                Debug.LogFormat("index: {0}, {1} {2} {3}", GetIndex1DFrom3D(pos.x, pos.y, pos.z), pos.x, pos.y, pos.z);
                throw e;
            }
        }

        public void SetBlockData(int x, int y, int z, BlockData data)
        {
            _blocks[GetIndex1DFrom3D(x, y, z)] = data;
        }

		public float BlockSize {
			get { 
				return _blockSize;
			}
		}
    }
}