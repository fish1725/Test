using UnityEngine;
using Vox.Core;

namespace Vox.Handler
{
    public class ChunkRenderGeometryHandler : ARenderGeometryHandler
	{
		private const string PoolEntryName = "Renderable";
		private readonly Chunk chunk;

		public ChunkRenderGeometryHandler(Chunk chunk, Material[] materials) : base(PoolEntryName, materials)
		{
			this.chunk = chunk;
		}

		/// <summary> Updates the chunk based on its contents </summary>
		public override void Build()
		{
			Globals.CubeMeshBuilder.Build(chunk);
		}

		public override void Commit()
		{
			Batcher.Commit(
                Vector3.zero,
                Quaternion.identity
#if DEBUG
				, chunk.ToString()
#endif
				);
		}
	}
}