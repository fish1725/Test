using UnityEngine;
using System.Collections;
using Vox.Core;

namespace Vox.DataProvider {

	public interface IDataProvider
	{
		BlockData GetBlockData(IBlockSet blockset, int x, int y, int z);
	}

}