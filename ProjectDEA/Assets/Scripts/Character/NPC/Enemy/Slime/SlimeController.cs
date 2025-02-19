using System;
using System.Collections.Generic;
using Character.NPC.State;
using UnityEngine;
using AnimationBool = Character.NPC.EnemyAnimHandler.AnimationBool;
using AnimationTrigger = Character.NPC.EnemyAnimHandler.AnimationTrigger;

namespace Character.NPC.Enemy.Slime
{
    public class SlimeController : NpcController
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
        	public AudioClip _hitAudio;
	        public float _searchUpPadding;
        }
        [SerializeField] private ParamAttack1 _attackParameters;
        [Serializable]
        public struct ParamEscape
        {
        	public float _searchOffSetFactor;
        	public float _searchRadius;
        	public LayerMask _targetLayer;
        	public float _escapeRatio;
	        public float _searchUpPadding;
        }
        [SerializeField] private ParamEscape _escapeParameters;
        
        private readonly List<IBattleSubState> _subStates = new();
        private readonly Dictionary<AnimationBool, int> _boolStateToHash = new()
        {
	        { AnimationBool.Moving, Animator.StringToHash("IsMoving") },
	        { AnimationBool.Rest, Animator.StringToHash("IsRest") }
        };
        private readonly Dictionary<AnimationTrigger, int> _triggerStateToHash = new()
        {
	        { AnimationTrigger.Attack1, Animator.StringToHash("IsAttack1") },
	        { AnimationTrigger.OnDamaged, Animator.StringToHash("OnDamaged") },
	        { AnimationTrigger.OnDead, Animator.StringToHash("OnDead") }
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
        	_subStates.Add(new SlimeAttack1(transform, EnemyAnimHandler, MovementControl, SoundHandler, _attackParameters));
        	_subStates.Add(new SlimeEscape(transform, EnemyAnimHandler, MovementControl, HealthComponent, _escapeParameters));
        }
    }
}
