using UnityEngine;

namespace Test.NPC
{
	[RequireComponent(typeof(AnimatorControl), typeof(MovementControl), typeof(HealthComponent))]
	[RequireComponent(typeof(NpcStatusComponent))]
	public class NpcController : MonoBehaviour
	{
		// 基底クラスで管理されるコンポーネント
		protected AnimatorControl AnimatorControl { get; private set; }
		protected MovementControl MovementControl { get; private set; }
		protected HealthComponent HealthComponent { get; private set; }
		protected NpcStatusComponent NpcStatusComponent { get; private set; }

		// 行動選択に関連する変数
		private protected ActionSelector ActionSelector;
		private IUtilityAction _currentAction;

		[SerializeField] private float _actionCooldown = 1.0f;
		private float _nextEvaluationTime;

		protected virtual void Start()
		{
			AnimatorControl = GetComponent<AnimatorControl>();
			MovementControl = GetComponent<MovementControl>();
			HealthComponent = GetComponent<HealthComponent>();
			NpcStatusComponent = GetComponent<NpcStatusComponent>();
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