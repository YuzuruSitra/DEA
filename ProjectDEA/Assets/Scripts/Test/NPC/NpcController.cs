using UnityEngine;

namespace Test.NPC
{
	[RequireComponent(typeof(AnimatorControl), typeof(MovementControl))]
	public class NpcController : MonoBehaviour
	{
		// 基底クラスで管理されるコンポーネント
		protected HealthComponent HealthComponent { get; private set; }
		protected AnimatorControl AnimatorControl { get; private set; }
		protected MovementControl MovementControl { get; private set; }

		// 行動選択に関連する変数
		protected ActionSelector ActionSelector { get; set; }
		private IUtilityAction _currentAction;

		[SerializeField] private float _actionCooldown = 1.0f;
		private float _nextEvaluationTime;

		protected virtual void Start()
		{
			HealthComponent = GetComponent<HealthComponent>();
			AnimatorControl = GetComponent<AnimatorControl>();
			MovementControl = GetComponent<MovementControl>();
		}

		private void Update()
		{
			if (Time.time >= _nextEvaluationTime)
			{
				_currentAction = ActionSelector.SelectBestAction();
				_nextEvaluationTime = Time.time + _actionCooldown;
			}
			_currentAction?.Execute(gameObject);
		}
	}
}