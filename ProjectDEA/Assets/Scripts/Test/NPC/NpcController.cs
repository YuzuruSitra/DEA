using System;
using Test.NPC.State;
using UnityEngine;

namespace Test.NPC
{
	[RequireComponent(typeof(AnimatorControl), typeof(MovementControl), typeof(HealthComponent))]
	[RequireComponent(typeof(NpcStatusComponent))]
	public class NpcController : MonoBehaviour, INpc
	{
		[SerializeField] private int _enemyID;
		public int EnemyID => _enemyID;
		[Serializable]
		public struct BattleStateParameters
		{
			public float _searchOffSetFactor;
			public float _searchRadius;
			public LayerMask _searchLayer;
		}
		[SerializeField] protected BattleStateParameters _battleStateParameters;
		// Roaming logic
		[Serializable]
		public struct RoamingParameters
		{
			public float _intervalTimeMax;
			public float _intervalTimeMin;
			public float _roamingSearchRange;
			public float _fullnessW;
			public float _staminaW;
			public float _bias;
		}
		[SerializeField] protected RoamingParameters _roamingParameters;
		
		[Serializable]
		public struct RestParameters
		{
			public float _restSearchRange;
			public float _bias;
			public float _targetAddStamina;
			public float _waitSleepTime;
		}
		[SerializeField] protected RestParameters _restParameters;
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
				var newState = ActionSelector.SelectBestAction();
				if (_currentAction != newState)
				{
					_currentAction?.ExitState();
					_currentAction = newState;
					_currentAction.EnterState();
				}
				_nextEvaluationTime = Time.time + _actionCooldown;
			}
			_currentAction?.Execute(gameObject);
		}
	}
}