using UnityEngine;
using System.Collections;
using Vox.Core;

namespace Vox.StateMachine {

	public class BlockSetStateGenerateData : BlockSetState
	{
		public BlockSetStateGenerateData() : base("GenerateDate") {
		} 

		public override void OnEnter (IBlockSet blockSet)
		{
			SetBlockData (blockSet);
		}

		public override void OnExit (IBlockSet blockSet)
		{
			
		}

		public void SetBlockData(IBlockSet blockSet) {
			var bounds = blockSet.GetBounds ();
			for (var i = bounds.Min.x; i < bounds.Max.x; i++) {
				for (var j = bounds.Min.y; j < bounds.Max.y; j++) {
					for (var k = bounds.Min.z; k < bounds.Max.z; k++) {
						var data = blockSet.DataProvider.GetBlockData (blockSet, i, j, k);
						blockSet.SetBlockData (i, j, k, data);
					}
				}
			}
		}
	}

}