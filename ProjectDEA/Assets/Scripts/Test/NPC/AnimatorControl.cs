using UnityEngine;

namespace Test.NPC
{
	public class AnimatorControl : MonoBehaviour
	{
		private Animator _animator;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
		}

		public void SetTrigger(string triggerName)
		{
			if (_animator != null)
			{
				_animator.SetTrigger(triggerName);
			}
		}

		public void PlayAnimation(string animationName)
		{
			if (_animator != null)
			{
				_animator.Play(animationName);
			}
		}

		public void SetBool(string parameterName, bool value)
		{
			if (_animator != null)
			{
				_animator.SetBool(parameterName, value);
			}
		}
	}
}