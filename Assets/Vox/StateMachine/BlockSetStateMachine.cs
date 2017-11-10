using UnityEngine;
using System.Collections;
using Vox.Core;
using System.Collections.Generic;
using System;

namespace Vox.StateMachine {
	
	public class BlockSetStateMachine
	{
		private Dictionary<string, BlockSetState> _states = new Dictionary<string, BlockSetState>();
		private BlockSetState _currentState;

		public void AddState(BlockSetState state) {
			if (_states.ContainsKey (state.Name)) {
				throw new UnityException (string.Format("BlockSet state machine already has state {0}", state.Name));
			}
			_states [state.Name] = state;
			state.StateMachine = this;
			if (_currentState == null) {
				_currentState = state;
			}
		}

		public BlockSetState GetState(string stateName) {
			BlockSetState state = null;
			_states.TryGetValue (stateName, out state);
			return state;
		}

		public void Update(IBlockSet blockSet) {
			if (_currentState != null) {
				foreach (var t in _currentState.Transitions) {
					if (t.condition (blockSet)) {
						_currentState.OnExit (blockSet);
						_currentState = t.toState;
						_currentState.OnEnter(blockSet);
					}
				}
			}
		}
	}

}