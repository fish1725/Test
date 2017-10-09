using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using g3;
using Vox.Core;
using Vox.Core.Blocks;

namespace Vox.Utils
{
    public class Voxelizer : MonoBehaviour
    {
        private IBlockSet _chunk;
        public float cellSize = 1;
        public MeshFilter meshFilter;

        private IntrTriangle3Triangle3 _tt = new IntrTriangle3Triangle3(new Triangle3d(), new Triangle3d());

        public Dictionary<Vector3, bool> VoxelizeSurface(Mesh mesh)
        {
            var triangles = mesh.triangles;
            var vertices = mesh.vertices;

            Bounds b = new Bounds();
            Dictionary<Vector3, bool> re = new Dictionary<Vector3, bool>();

            for (var i = 0; i < triangles.Length; i += 3)
            {
                var p1 = meshFilter.transform.TransformPoint(vertices[triangles[i]]);
                var p2 = meshFilter.transform.TransformPoint(vertices[triangles[i + 1]]);
                var p3 = meshFilter.transform.TransformPoint(vertices[triangles[i + 2]]);

                b.center = (p1 + p2 + p3) / 3;
                b.size = Vector3.zero;
                b.Encapsulate(p1);
                b.Encapsulate(p2);
                b.Encapsulate(p3);

                for (float x = Mathf.FloorToInt(b.min.x / cellSize) * cellSize, maxx = Mathf.CeilToInt(b.max.x / cellSize) * cellSize; x <= maxx; x += cellSize)
                {
                    for (float y = Mathf.FloorToInt(b.min.y / cellSize) * cellSize, maxy = Mathf.CeilToInt(b.max.y / cellSize) * cellSize; y <= maxy; y += cellSize)
                    {
                        for (float z = Mathf.FloorToInt(b.min.z / cellSize) * cellSize, maxz = Mathf.CeilToInt(b.max.z / cellSize) * cellSize; z <= maxz; z += cellSize)
                        {
                            var center = new Vector3(x, y, z);
                            if (Intersect(center, cellSize, p1, p2, p3))
                            {
                                re[center] = true;
                            }
                        }
                    }
                }
            }
            return re;
        }

        public void Materlize(Dictionary<Vector3, bool> data)
        {
            Globals.blockDatabase.SetBlock(1, new ColoredBlock(new BlockConfig { Type = 1, Name = "CB", Solid = true }));
			Globals.blockDatabase.SetBlock(0, new Block(new BlockConfig { Type = 0, Name = "Air" }));
            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int minY = int.MaxValue;
            int maxY = int.MinValue;
            int minZ = int.MaxValue;
            int maxZ = int.MinValue;

            foreach (var item in data)
            {
                if (item.Value)
                {
                    if(minX > (int)item.Key.x) {
                        minX = (int)item.Key.x;
                    }
                    if(maxX < (int)item.Key.x) {
                        maxX = (int)item.Key.x;
                    }
                    if(minY > (int)item.Key.y) {
                        minY = (int)item.Key.y;
                    }
                    if(maxY < (int)item.Key.y) {
                        maxY = (int)item.Key.y;
                    }
                    if(minZ > (int)item.Key.z) {
                        minZ = (int)item.Key.z;
                    }
                    if(maxZ < (int)item.Key.z) {
                        maxZ = (int)item.Key.z;
                    }
                }
			}
            _chunk = new VoxObject(minX, maxX + 1, minY, maxY + 1, minZ, maxZ + 1);
			foreach (var item in data)
			{
				if (item.Value)
				{
                    _chunk.SetBlockData(item.Key, new BlockData(1, true));
				}
			}
			_chunk.RenderGeometryHandler.Build();
			_chunk.RenderGeometryHandler.Commit();
        }

        public bool Intersect(Vector3 center, float size, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var x1 = center.x - size;
            var x2 = center.x + size;
            var y1 = center.y - size;
            var y2 = center.y + size;
            var z1 = center.z - size;
            var z2 = center.z + size;

            Triangle3d t1;

            t1.V0 = p1;
            t1.V1 = p2;
            t1.V2 = p3;

            _tt.Triangle0 = t1;

            Triangle3d t2;

            t2.V0 = new Vector3(x1, y1, z1);
            t2.V1 = new Vector3(x1, y2, z1);
            t2.V2 = new Vector3(x2, y1, z1);
            _tt.Triangle1 = t2;
            if (_tt.Test())
                return true;

            t2.V0 = new Vector3(x1, y1, z2);
            t2.V1 = new Vector3(x1, y2, z2);
            t2.V2 = new Vector3(x2, y1, z2);
            _tt.Triangle1 = t2;
            if (_tt.Test())
                return true;

            t2.V0 = new Vector3(x1, y2, z1);
            t2.V1 = new Vector3(x2, y2, z1);
            t2.V2 = new Vector3(x2, y1, z1);
            _tt.Triangle1 = t2;
            if (_tt.Test())
                return true;

            t2.V0 = new Vector3(x1, y2, z2);
            t2.V1 = new Vector3(x2, y2, z2);
            t2.V2 = new Vector3(x2, y1, z2);
            _tt.Triangle1 = t2;
            if (_tt.Test())
                return true;

            t2.V0 = new Vector3(x1, y2, z1);
            t2.V1 = new Vector3(x1, y2, z2);
            t2.V2 = new Vector3(x1, y1, z1);
            _tt.Triangle1 = t2;
            if (_tt.Test())
                return true;

            t2.V0 = new Vector3(x2, y2, z1);
            t2.V1 = new Vector3(x2, y2, z2);
            t2.V2 = new Vector3(x2, y1, z1);
            _tt.Triangle1 = t2;
            if (_tt.Test())
                return true;

            t2.V0 = new Vector3(x1, y2, z2);
            t2.V1 = new Vector3(x1, y1, z2);
            t2.V2 = new Vector3(x1, y1, z1);
            _tt.Triangle1 = t2;
            if (_tt.Test())
                return true;

            t2.V0 = new Vector3(x2, y2, z2);
            t2.V1 = new Vector3(x2, y1, z2);
            t2.V2 = new Vector3(x2, y1, z1);
            _tt.Triangle1 = t2;
            if (_tt.Test())
                return true;

            t2.V0 = new Vector3(x1, y1, z2);
            t2.V1 = new Vector3(x1, y1, z1);
            t2.V2 = new Vector3(x2, y1, z2);
            _tt.Triangle1 = t2;
            if (_tt.Test())
                return true;

            t2.V0 = new Vector3(x1, y2, z2);
            t2.V1 = new Vector3(x1, y2, z1);
            t2.V2 = new Vector3(x2, y2, z2);
            _tt.Triangle1 = t2;
            if (_tt.Test())
                return true;

            t2.V0 = new Vector3(x2, y1, z1);
            t2.V1 = new Vector3(x1, y1, z1);
            t2.V2 = new Vector3(x2, y1, z2);
            _tt.Triangle1 = t2;
            if (_tt.Test())
                return true;

            t2.V0 = new Vector3(x2, y2, z1);
            t2.V1 = new Vector3(x1, y2, z1);
            t2.V2 = new Vector3(x2, y2, z2);
            _tt.Triangle1 = t2;
            if (_tt.Test())
                return true;

            return false;
        }

        [ContextMenu("Voxelize")]
        public void Voxelize()
        {
            if (meshFilter != null)
            {
                var data = VoxelizeSurface(meshFilter.sharedMesh);
                Materlize(data);
            }
        }
    }
}