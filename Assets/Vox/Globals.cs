using F.Memory;
using g3;
using UnityEngine;
using Vox.Builder.Geometry;
using Vox.Common;

namespace Vox
{
    public class Globals
    {
        public static readonly ArrayPoolCollection<BlockFace> blockFacePool =
            new ArrayPoolCollection<BlockFace>(128);

		public static readonly ArrayPoolCollection<Vector2> Vector2ArrayPool =
			new ArrayPoolCollection<Vector2>(128);

		public static readonly ArrayPoolCollection<Vector3> Vector3ArrayPool =
			new ArrayPoolCollection<Vector3>(128);
        
		public static readonly ArrayPoolCollection<Vector3i> Vector3iArrayPool =
			new ArrayPoolCollection<Vector3i>(128);
        
		public static readonly ArrayPoolCollection<Color32> Color32ArrayPool =
			new ArrayPoolCollection<Color32>(128);

		public static readonly ArrayPoolCollection<bool> BoolArrayPool =
			new ArrayPoolCollection<bool>(128);

        public static readonly ArrayPoolCollection<VertexData> VertexDataArrayPool =
            new ArrayPoolCollection<VertexData>(128);

		public static readonly ObjectPool<Mesh> MeshPool =
			new ObjectPool<Mesh>(m => new Mesh(), 128, true);

        public static readonly CubeMeshGeometryBuilder CubeMeshBuilder = new CubeMeshGeometryBuilder();
    }
}