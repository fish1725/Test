using UnityEngine;
using System.Collections;
using Vox.Core;
using System.Collections.Generic;
using System;

namespace Vox.StateMachine {

	public abstract class BlockSetState
	{
		private List<BlockSetTransition> _transitions = new List<BlockSetTransition>();

		public string Name { get; set; }
		public BlockSetStateMachine StateMachine { get; set; }

		public virtual void OnEnter(IBlockSet blockSet) {
			
		}

		public virtual void OnExit(IBlockSet blockSet) {
			
		}

		public BlockSetState(string stateName) {
			Name = stateName;
		}

		public void AddTransition(string toStateName, Func<IBlockSet, bool> condition) {
			var toState = StateMachine.GetState (toStateName);
			var t = new BlockSetTransition (this, toState, condition);
			_transitions.Add (t);
		}


		public List<BlockSetTransition> Transitions {
			get {
				return _transitions;
			}
		}
	}

}