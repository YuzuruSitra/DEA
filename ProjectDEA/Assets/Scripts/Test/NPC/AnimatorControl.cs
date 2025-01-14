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
			var speedRatio = _agent.velocity.magnitude / _agent.speed;
			_animator.SetFloat(_speedRatio, speedRatio);
		}

		public void SetAnimParameter(AnimationState newState)
		{
			if (newState == _currentState)
			{
				if (newState != AnimationState.Attack) return;
				var clipInfo = _animator.GetCurrentAnimatorClipInfo(0)[0];
				_animator.Play(clipInfo.clip.name, 0, 0f);
				return;
			}
			_animator.SetBool(_stateToHashMap[_currentState], false);
			_currentState = newState;
			_animator.SetBool(_stateToHashMap[newState], true);
		}
	}
}