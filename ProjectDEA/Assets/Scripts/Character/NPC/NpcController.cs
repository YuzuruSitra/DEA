using System;
using System.Collections;
using Character.NPC.State;
using Manager.Audio;
using Mission;
using UnityEngine;
using UnityEngine.AI;
using AnimationTrigger = Character.NPC.EnemyAnimHandler.AnimationTrigger;

namespace Character.NPC
{
	[RequireComponent(typeof(EnemyHpGaugeHandler))]
	public class NpcController : MonoBehaviour
	{
		[SerializeField] private int _enemyID;
		[SerializeField] private float _actionCooldown;
		[SerializeField] private float _removedTime;
		[SerializeField] private float _maxHealth;
		[SerializeField] private float _staminaChangeSecond;
		[SerializeField] private float _fullnessChangeSecond;
		private GameEventManager _gameEventManager;
		private NavMeshAgent _agent;
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
		protected EnemyAnimHandler EnemyAnimHandler { get; private set; }
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
			var animator = GetComponent<Animator>();
			_agent = GetComponent<NavMeshAgent>();
			EnemyAnimHandler = new EnemyAnimHandler(animator);
			MovementControl = new MovementControl(_agent);
			HealthComponent = new HealthComponent(_maxHealth);
			NpcStatusComponent = new NpcStatusComponent(_staminaChangeSecond, _fullnessChangeSecond);
			EnemyHpGaugeHandler = GetComponent<EnemyHpGaugeHandler>();
			
			HealthComponent.OnDeath += OnDeath;
			EnemyHpGaugeHandler.InitialSet(HealthComponent.MaxHealth, HealthComponent.CurrentHealth);
			HealthComponent.OnHealthChanged += EnemyHpGaugeHandler.ChangeGauge;
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
			
			EnemyAnimHandler.CalcSpeedRatio(_agent.velocity.magnitude, _agent.speed);
			
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
			EnemyAnimHandler.OnTriggerAnim(AnimationTrigger.OnDamaged);
		}
		
		private void OnDeath()
		{
			_gameEventManager.EnemyDefeated(_enemyID);
			MovementControl.ChangeMove(false);
			EnemyAnimHandler.OnTriggerAnim(AnimationTrigger.OnDead);
			StartCoroutine(DelayedDestroy());
		}
		
		private IEnumerator DelayedDestroy()
		{
			yield return new WaitForSeconds(_removedTime);
			Destroy(gameObject);
		}
	}
}