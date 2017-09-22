using UnityEngine;
using Vox.Batcher;

namespace Vox.Handler
{
    public abstract class ARenderGeometryHandler : IGeometryHandler
	{
		public RenderGeometryBatcher Batcher { get; private set; }

		protected ARenderGeometryHandler(string prefabName, Material[] materials)
		{
			Batcher = new RenderGeometryBatcher(prefabName, materials);
		}

		public void Reset()
		{
			Batcher.Reset();
		}

		public abstract void Build();

		public abstract void Commit();
	}
}
