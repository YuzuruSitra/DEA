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
			OnDamaged,
			OnDead
		}
		
		private readonly int _speedRatio = Animator.StringToHash("SpeedRatio");
		private readonly Dictionary<AnimationBool, int> _boolStateToHash = new()
		{
			{ AnimationBool.Moving, Animator.StringToHash("IsMoving") },
			{ AnimationBool.Rest, Animator.StringToHash("IsRest") }
		};
		private readonly Dictionary<AnimationTrigger, int> _triggerStateToHash = new()
		{
			{ AnimationTrigger.Attack, Animator.StringToHash("IsAttack1") },
			{ AnimationTrigger.OnDamaged, Animator.StringToHash("OnDamaged") },
			{ AnimationTrigger.OnDead, Animator.StringToHash("OnDead") }
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
			var stateHash = _triggerStateToHash[newState];
			var currentState = _animator.GetCurrentAnimatorStateInfo(0);
			// すでに同じアニメーションが再生中なら最初から再生
			if (currentState.shortNameHash == stateHash)
			{
				_animator.Play(stateHash, 0, 0.0f);
			}
			else
			{
				_animator.SetTrigger(stateHash);
			}
		}

	}
}