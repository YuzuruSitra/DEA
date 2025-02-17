using System;
using System.Collections.Generic;
using Character.NPC.State;
using UnityEngine;

namespace Character.NPC.Enemy.Dragon
{
	public class DragonController : NpcController
	{
		[Serializable]
		public struct ParamAttack1
		{
			public float _searchOffSetFactor;
			public float _searchRadius;
			public LayerMask _targetLayer;
			public float _attackOffSetFactor;
			public float _attackRadius;
			public float _takeDamageWait;
			public float _attackDuration;
			public int _attackDamage;
			public float _pushPower;
			public float _stopFactor;
			public float _screamTime;
			public float _screamWaitTime;
			public AudioClip _screamAudio;
			public AudioClip _hitAudio;
		}
		[SerializeField] private ParamAttack1 _paramAttack1;
		[Serializable]
		public struct ParamEscape
		{
			public float _searchOffSetFactor;
			public float _searchRadius;
			public LayerMask _targetLayer;
			public float _escapeRatio;
		}
		[SerializeField] private ParamEscape _paramEscape;
		
		private readonly List<IBattleSubState> _subStates = new();
		
		protected override void Start()
		{
			base.Start();
			InitializeSubStates();
			// ActionSelector にドラゴン固有のアクションを追加
			ActionSelector = new ActionSelector(new List<IUtilityAction>
			{
				new RestAction(transform, AnimatorControl, MovementControl, NpcStatusComponent, _restParameters),
				new RoamingAction(transform, AnimatorControl, MovementControl, NpcStatusComponent, _roamingParameters),
				new BattleState(transform, _subStates, _battleStateParameters)
			});
		}

		private void InitializeSubStates()
		{
			_subStates.Add(new DragonAttack1(transform, AnimatorControl, MovementControl, SoundHandler, _paramAttack1));
			_subStates.Add(new DragonEscape(transform, AnimatorControl, MovementControl, HealthComponent, _paramEscape));
		}
		
	}
}