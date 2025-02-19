using System;
using System.Collections.Generic;
using Character.NPC.State;
using UnityEngine;

namespace Character.NPC.Enemy.Golem
{
	public class GolemController : NpcController
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
			public float _searchUpPadding;
			public AudioClip _hitAudio;
		}
		[SerializeField] private ParamAttack1 _paramAttack1;
		private readonly List<IBattleSubState> _subStates = new();
		
		private readonly Dictionary<EnemyAnimHandler.AnimationBool, int> _boolStateToHash = new()
		{
			{ EnemyAnimHandler.AnimationBool.Moving, Animator.StringToHash("IsMoving") },
			{ EnemyAnimHandler.AnimationBool.Rest, Animator.StringToHash("IsRest") }
		};
		private readonly Dictionary<EnemyAnimHandler.AnimationTrigger, int> _triggerStateToHash = new()
		{
			{ EnemyAnimHandler.AnimationTrigger.Attack1, Animator.StringToHash("IsAttack1") },
			{ EnemyAnimHandler.AnimationTrigger.OnDamaged, Animator.StringToHash("OnDamaged") },
			{ EnemyAnimHandler.AnimationTrigger.OnDead, Animator.StringToHash("OnDead") }
		};
		
		protected override void Start()
		{
			base.Start();
			InitializeSubStates();
			// ActionSelector にドラゴン固有のアクションを追加
			ActionSelector = new ActionSelector(new List<IUtilityAction>
			{
				new RestAction(transform, EnemyAnimHandler, MovementControl, NpcStatusComponent, _restParameters),
				new RoamingAction(transform, EnemyAnimHandler, MovementControl, NpcStatusComponent, _roamingParameters),
				new BattleState(transform, _subStates, _battleStateParameters)
			});
			// アニメーションのハッシュセット登録
			EnemyAnimHandler.SetHashSet(_boolStateToHash, _triggerStateToHash);
		}

		private void InitializeSubStates()
		{
			_subStates.Add(new GolemAttack1(transform, EnemyAnimHandler, MovementControl, SoundHandler, _paramAttack1));
		}
	}
}
