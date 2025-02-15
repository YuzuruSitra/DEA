using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Test.NPC
{
	public class AnimatorControl : MonoBehaviour
	{
		private Animator _animator;
		private NavMeshAgent _agent;

		public enum AnimationBool
		{
			None,
			Moving,
			Rest
		}
		private AnimationBool _currentState = AnimationBool.None;

		public enum AnimationTrigger
		{
			Attack,
			OnDamaged
		}
		
		private readonly int _speedRatio = Animator.StringToHash("SpeedRatio");
		private readonly Dictionary<AnimationBool, int> _boolStateToHash = new()
		{
			{ AnimationBool.Moving, Animator.StringToHash("IsMoving") },
			{ AnimationBool.Rest, Animator.StringToHash("IsRest") }
		};
		private readonly Dictionary<AnimationTrigger, int> _triggerStateToHash = new()
		{
			{ AnimationTrigger.Attack, Animator.StringToHash("IsAttack") },
			{ AnimationTrigger.OnDamaged, Animator.StringToHash("OnDamaged") }
		};

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_agent = GetComponent<NavMeshAgent>();
		}

		private void Update()
		{
			if (_currentState != AnimationBool.Moving) return;
			var speedRatio = _agent.velocity.magnitude / _agent.speed;
			_animator.SetFloat(_speedRatio, speedRatio);
		}

		public void ChangeAnimBool(AnimationBool newState)
		{
			if (newState == _currentState) return;
			if (_currentState != AnimationBool.None) _animator.SetBool(_boolStateToHash[_currentState], false);
			_animator.SetBool(_boolStateToHash[newState], true);
			_currentState = newState;
		}

		public void OnTriggerAnim(AnimationTrigger newState)
		{
			_animator.SetTrigger(_triggerStateToHash[newState]);
		}

	}
}