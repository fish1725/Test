using UnityEngine;
using System.Collections;
using System;
using Vox.Core;

namespace Vox.StateMachine {

	public class BlockSetTransition
	{
		private BlockSetState _fromState;
		private BlockSetState _toState;

		private Func<IBlockSet, bool> _condition;

		public BlockSetTransition(BlockSetState fromState, BlockSetState toState, Func<IBlockSet, bool> condition) {
			_fromState = fromState;
			_toState = toState;
			_condition = condition;
		}

		public BlockSetState fromState { get { return _fromState; } }
		public BlockSetState toState { get { return _toState; } }
		public Func<IBlockSet, bool> condition { get { return _condition; } }

	}

}