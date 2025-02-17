using System;
using System.Collections.Generic;
using Character.NPC.State;
using UnityEngine;

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
        }
        [SerializeField] private ParamAttack1 _attackParameters;
        [Serializable]
        public struct ParamEscape
        {
        	public float _searchOffSetFactor;
        	public float _searchRadius;
        	public LayerMask _targetLayer;
        	public float _escapeRatio;
        }
        [SerializeField] private ParamEscape _escapeParameters;
        
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
        	_subStates.Add(new SlimeAttack1(transform, AnimatorControl, MovementControl, SoundHandler, _attackParameters));
        	_subStates.Add(new SlimeEscape(transform, AnimatorControl, MovementControl, HealthComponent, _escapeParameters));
        }
    }
}
