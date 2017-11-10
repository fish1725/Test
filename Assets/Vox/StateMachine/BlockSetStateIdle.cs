using UnityEngine;
using System.Collections;

namespace Vox.StateMachine {

	public class BlockSetStateIdle : BlockSetState
	{
		public BlockSetStateIdle() : base("Idle") {
			AddTransition ("GenerateData", (blockSet) => {
			});
		}

		public override void OnEnter (Vox.Core.IBlockSet blockSet)
		{
			
		}

		public override void OnExit (Vox.Core.IBlockSet blockSet)
		{
			
		}
	}

}