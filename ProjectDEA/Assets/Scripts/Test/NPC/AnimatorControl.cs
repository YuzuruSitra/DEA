using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Test.NPC
{
	public class AnimatorControl : MonoBehaviour
	{
		private Animator _animator;
		private NavMeshAgent _agent;

		public enum AnimationState
		{
			Moving,
			Attack,
			Rest
		}
		private AnimationState _currentState;
		
		private readonly int _speedRatio = Animator.StringToHash("SpeedRatio");
		private readonly Dictionary<AnimationState, int> _stateToHashMap = new()
		{
			{ AnimationState.Moving, Animator.StringToHash("IsMoving") },
			{ AnimationState.Attack, Animator.StringToHash("IsAttack") },
			{ AnimationState.Rest, Animator.StringToHash("IsRest") }
		};

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_agent = GetComponent<NavMeshAgent>();
		}

		private void Update()
		{
			if (_currentState != AnimationState.Moving) return;
			_animator.SetFloat(_speedRatio, _agent.velocity.magnitude);
		}

		public void SetAnimParameter(AnimationState newState)
		{
			_animator.SetBool(_stateToHashMap[_currentState], false);
			_currentState = newState;
			_animator.SetBool(_stateToHashMap[newState], true);
		}
	}
}