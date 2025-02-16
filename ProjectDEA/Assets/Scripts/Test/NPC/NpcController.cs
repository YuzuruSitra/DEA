using System;
using System.Collections;
using Manager.Audio;
using Mission;
using Test.NPC.State;
using UnityEngine;
using AnimationTrigger = Test.NPC.AnimatorControl.AnimationTrigger;

namespace Test.NPC
{
	[RequireComponent(typeof(AnimatorControl), typeof(MovementControl), typeof(HealthComponent))]
	[RequireComponent(typeof(NpcStatusComponent)), RequireComponent(typeof(EnemyHpGaugeHandler))]
	public class NpcController : MonoBehaviour
	{
		[SerializeField] private int _enemyID;
		[SerializeField] private float _actionCooldown = 1.0f;
		[SerializeField] private float _removedTime;
		private GameEventManager _gameEventManager;
		
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
		private EnemyHpGaugeHandler EnemyHpGaugeHandler { get; set; }
		private protected SoundHandler SoundHandler;
		// 行動選択に関連する変数
		private protected ActionSelector ActionSelector;
		private IUtilityAction _currentAction;
		private float _nextEvaluationTime;

		protected virtual void Start()
		{
			AnimatorControl = GetComponent<AnimatorControl>();
			MovementControl = GetComponent<MovementControl>();
			HealthComponent = GetComponent<HealthComponent>();
			EnemyHpGaugeHandler = GetComponent<EnemyHpGaugeHandler>();
			
			HealthComponent.OnDeath += OnDeath;
			EnemyHpGaugeHandler.InitialSet(HealthComponent.MaxHealth, HealthComponent.CurrentHealth);
			HealthComponent.OnHealthChanged += EnemyHpGaugeHandler.ChangeGauge;
			NpcStatusComponent = GetComponent<NpcStatusComponent>();
			_gameEventManager = GameObject.FindWithTag("GameEventManager").GetComponent<GameEventManager>();
			SoundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
		}

		private void OnDestroy()
		{
			HealthComponent.OnDeath -= OnDeath;
			HealthComponent.OnHealthChanged -= EnemyHpGaugeHandler.ChangeGauge;
		}

		private void Update()
		{
			if (HealthComponent.CurrentHealth <= 0) return;
			
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
		
		public void OnGetDamage(int damage)
		{
			HealthComponent.TakeDamage(damage);
			AnimatorControl.OnTriggerAnim(AnimationTrigger.OnDamaged);
		}
		
		private void OnDeath()
		{
			_gameEventManager.EnemyDefeated(_enemyID);
			AnimatorControl.OnTriggerAnim(AnimationTrigger.OnDead);
			StartCoroutine(DelayedDestroy());
		}
		
		private IEnumerator DelayedDestroy()
		{
			yield return new WaitForSeconds(_removedTime);
			Destroy(gameObject);
		}
	}
}