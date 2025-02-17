using System.Collections.Generic;
using UnityEngine;

namespace Character.NPC
{
	public class EnemyAnimHandler
	{
		private readonly Animator _animator;

		public enum AnimationBool
		{
			None,
			Moving,
			Rest
		}
		private AnimationBool _currentState = AnimationBool.None;

		public enum AnimationTrigger
		{
			Attack1,
			OnDamaged,
			OnScream,
			OnDead
		}
		
		private readonly int _speedRatio = Animator.StringToHash("SpeedRatio");
		private Dictionary<AnimationBool, int> _boolStateToHash;
		private Dictionary<AnimationTrigger, int> _triggerStateToHash;

		public EnemyAnimHandler(Animator animator)
		{
			_animator = animator;
		}

		public void SetHashSet(Dictionary<AnimationBool, int> boolStateToHash, Dictionary<AnimationTrigger, int> triggerStateToHash)
		{
			_boolStateToHash = boolStateToHash;
			_triggerStateToHash = triggerStateToHash;
		}

		public void CalcSpeedRatio(float velocity, float speed)
		{
			if (_currentState != AnimationBool.Moving) return;
			var speedRatio = velocity / speed;
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