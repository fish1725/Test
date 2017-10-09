using UnityEngine;
using System.Collections;
using Vox.Core;
using g3;
using Vox.Utils;

namespace Vox.Builder.RenderGeometry
{
    public struct BlockFace
    {
        public Block block;
        public Vector3i direction;
        public int materialId;
        public BlockFaceAOData aoData;
    }
}