using System;
using System.Collections.Generic;
using Test.NPC.State;
using UnityEngine;

namespace Test.NPC.Dragon
{
	public class DragonController : NpcController
	{
		[Serializable]
		public struct BattleStateParameters
		{
			public float _searchOffSetFactor;
			public float _searchRadius;
			public LayerMask _searchLayer;
		}
		[SerializeField] private BattleStateParameters _battleStateParameters;
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
		[SerializeField] private RoamingParameters _roamingParameters;
		
		[Serializable]
		public struct RestParameters
		{
			public float _restSearchRange;
			public float _bias;
			public float _targetAddStamina;
			public float _waitSleepTime;
		}
		[SerializeField] private RestParameters _restParameters;
		[Serializable]
		public struct AttackParameters
		{
			public float _searchOffSetFactor;
			public float _searchRadius;
			public LayerMask _targetLayer;
			public float _attackOffSetFactor;
			public float _attackRadius;
			public float _attackDuration;
			public float _attackDamage;
			public float _stopFactor;
		}
		[SerializeField] private AttackParameters _attackParameters;
		[Serializable]
		public struct EscapeParameters
		{
			public float _searchOffSetFactor;
			public float _searchRadius;
			public LayerMask _targetLayer;
			public float _escapeRatio;
		}
		[SerializeField] private EscapeParameters _escapeParameters;
		
		private readonly List<IBattleSubState> _subStates = new();
		
		protected override void Start()
		{
			base.Start();
			InitializeSubStates();
			// ActionSelector にドラゴン固有のアクションを追加
			ActionSelector = new ActionSelector(new List<IUtilityAction>
			{
				//new AttackAction(transform, AnimatorControl, MovementControl, _attackParameters),
				new RestAction(transform, AnimatorControl, MovementControl, NpcStatusComponent, _restParameters),
				new RoamingAction(transform, AnimatorControl, MovementControl, NpcStatusComponent, _roamingParameters),
				new BattleState(transform, _subStates, _battleStateParameters)
			});
		}

		private void InitializeSubStates()
		{
			// _subStates = List<IBattleSubState>();
			_subStates.Add(new AttackAction(transform, AnimatorControl, MovementControl, _attackParameters));
			_subStates.Add(new EscapeAction(transform, AnimatorControl, MovementControl, HealthComponent, _escapeParameters));
		}
		
	}
}