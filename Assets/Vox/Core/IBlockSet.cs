using g3;
using Vox.Handler;

namespace Vox.Core
{
    public interface IBlockSet
    {
        BlockData GetBlockData(Vector3i pos);
        BlockData GetBlockData(int x, int y, int z);
        void SetBlockData(Vector3i pos, BlockData data);
        void SetBlockData(int x, int y, int z, BlockData data);
		Block GetBlock(Vector3i pos);
		Block GetBlock(int x, int y, int z);
        int GetIndex1DFrom3D(int x, int y, int z);
        int GetBlockCount();
        AxisAlignedBox3i GetBounds();
        ARenderGeometryHandler RenderGeometryHandler { get; }
		float BlockSize { get; }
    }
}