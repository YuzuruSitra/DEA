using System.Collections.Generic;
using Character.NPC.Dragon;
using Character.Player;
using Manager.Audio;
using Test;
using UnityEngine;
using static Character.NPC.AnimatorControl;

namespace Character.NPC.State
{
    public class AttackAction : IBattleSubState
    {
        private readonly AnimatorControl _animatorControl;
        private readonly MovementControl _movementControl;
        private readonly SoundHandler _soundHandler;
        private readonly Transform _agent;

        private readonly float _searchOffSetFactor;
        private readonly float _searchRadius;
        private readonly LayerMask _searchLayer;
        private readonly float _attackOffSetFactor;
        private readonly float _attackRadius;
        private readonly float _takeDamageWait;
        private readonly float _attackDuration;
        private readonly int _attackDamage;
        private readonly float _pushPower;
        private readonly float _stopFactor;
        private readonly float _screamTime;
        private readonly float _screamWaitTime;
        private readonly AudioClip _screamAudio;
        private readonly AudioClip _hitAudio;

        private Transform _target;
        private readonly DebugDrawCd _debugDrawCd;
        private readonly HashSet<Collider> _hitTargets = new();
        private bool _isOnCooldown;
        private float _attackCt;
        private float _attackTimer;
        private bool _isAttacking;

        private const float DamageCheckTime = 0.5f;
        private readonly Collider[] _searchResults = new Collider[1];
        private readonly Collider[] _attackResults = new Collider[1];
        private float _remainTakeDamageWait;
        private float _remainScreamTime;
        private float _remainScreamWaitTime;
        private bool _isScream;

        public AttackAction(Transform agent, AnimatorControl animatorControl, MovementControl movementControl, SoundHandler soundHandler, DragonController.AttackParameters attackParameters)
        {
            _agent = agent;
            _animatorControl = animatorControl;
            _movementControl = movementControl;
            _soundHandler = soundHandler;
            _searchOffSetFactor = attackParameters._searchOffSetFactor;
            _searchRadius = attackParameters._searchRadius;
            _searchLayer = attackParameters._targetLayer;
            _attackOffSetFactor = attackParameters._attackOffSetFactor;
            _attackRadius = attackParameters._attackRadius;
            _takeDamageWait = attackParameters._takeDamageWait;
            _attackDuration = attackParameters._attackDuration;
            _attackDamage = attackParameters._attackDamage;
            _pushPower = attackParameters._pushPower;
            _stopFactor = attackParameters._stopFactor;
            _screamTime = attackParameters._screamTime;
            _screamWaitTime = attackParameters._screamWaitTime;
            _screamAudio = attackParameters._screamAudio;
            _hitAudio = attackParameters._hitAudio;
            _debugDrawCd = new DebugDrawCd();
        }
        
        public float CalculateUtility()
        {
            return 0.5f;
        }
        
        public void EnterState(Transform target)
        {
            _isOnCooldown = false;
            _attackCt = 0;
            _attackTimer = DamageCheckTime;
            _movementControl.ChangeMove(true);
            _movementControl.MoveTo(target.position);
            _animatorControl.ChangeAnimBool(AnimationBool.Moving);
            _isAttacking = false;
            _remainScreamTime = _screamTime;
            _remainScreamWaitTime = _screamWaitTime;
            _isScream = false;
        }

        public void Execute()
        {
            DrawDebugSpheres();
            // 咆哮待機
            if (_remainScreamWaitTime >= 0)
            {
                _remainScreamWaitTime -= Time.deltaTime;
                return;
            }
            // 咆哮
            if (_remainScreamTime >= 0)
            {
                if (!_isScream)
                {
                    _animatorControl.OnTriggerAnim(AnimationTrigger.OnScream);
                    _soundHandler.PlaySe(_screamAudio);
                    _isScream = true;
                }
                _movementControl.ChangeMove(false);
                _remainScreamTime -= Time.deltaTime;
                return;
            }

            if (HandleCooldown()) return;
            
            if (_target == null || !IsTargetValid())
            {
                _target = FindTarget(_agent.position + _agent.forward * _searchOffSetFactor, _searchRadius);
                return;
            }

            if (!IsTargetInRange())
            {
                _movementControl.ChangeMove(true);
                _movementControl.MoveTo(_target.position);
                _animatorControl.ChangeAnimBool(AnimationBool.Moving);
                if (_isAttacking)
                {
                    ResetAttack();
                }
                return;
            }
          
            ExecuteDamageLoop();
        }

        public void ExitState()
        {
            _movementControl.ChangeMove(true);
            ResetAttack();
        }

        private void DrawDebugSpheres()
        {
            var agentPos = _agent.position;
            agentPos.y += BattleState.UpPadding;
            _debugDrawCd.DebugDrawSphere(agentPos + _agent.forward * _searchOffSetFactor, _searchRadius, Color.blue);
            _debugDrawCd.DebugDrawSphere(agentPos + _agent.forward * _attackOffSetFactor, _attackRadius, Color.red);
        }

        private bool HandleCooldown()
        {
            if (!_isOnCooldown) return false;

            _attackCt -= Time.deltaTime;
            if (_attackCt <= 0)
            {
                _isOnCooldown = false;
            }
            return true;
        }

        private Transform FindTarget(Vector3 origin, float radius)
        {
            origin.y += BattleState.UpPadding;
            var count = Physics.OverlapSphereNonAlloc(origin, radius, _searchResults, _searchLayer, QueryTriggerInteraction.Ignore);
            return (count > 0 && _searchResults[0] != null) ? _searchResults[0].transform : null;
        }

        private bool IsTargetValid()
        {
            return FindTarget(_agent.position + _agent.forward * _searchOffSetFactor, _searchRadius) != null;
        }

        private bool IsTargetInRange()
        {
            return FindTarget(_agent.position + _agent.forward * (_attackOffSetFactor * _stopFactor), _attackRadius) != null;
        }

        private void ExecuteDamageLoop()
        {
            if (!_isAttacking)
            {
                _movementControl.ChangeMove(false);
                _animatorControl.OnTriggerAnim(AnimationTrigger.Attack);
                _isAttacking = true;
                _remainTakeDamageWait = _takeDamageWait;
            }

            if (_remainTakeDamageWait >= 0)
            {
                _remainTakeDamageWait -= Time.deltaTime;
                return;
            }

            if (_attackTimer > 0)
            {
                ApplyDamageToTargets();
                _attackTimer -= Time.deltaTime;
                return;
            }
            ResetAttack();
        }

        private void ResetAttack()
        {
            _hitTargets.Clear();
            _isOnCooldown = true;
            _attackCt = _attackDuration;
            _attackTimer = DamageCheckTime;
            _animatorControl.ChangeAnimBool(AnimationBool.Moving);
            _isAttacking = false;
        }

        private void ApplyDamageToTargets()
        {
            var origin = _agent.position;
            origin.y += BattleState.UpPadding;
            var size = Physics.OverlapSphereNonAlloc(origin + _agent.forward * _attackOffSetFactor, _attackRadius, _attackResults, _searchLayer, QueryTriggerInteraction.Ignore);
            if (size == 0) return;
            var collider = _attackResults[0];
            if (_hitTargets.Contains(collider)) return;
            if (!collider.TryGetComponent(out PlayerClasHub playerClasHub)) return;
            playerClasHub.PlayerMover.LaunchPushForceMove(_agent.forward.normalized, _pushPower);
            playerClasHub.PlayerHpHandler.ReceiveDamage(_attackDamage);
            _soundHandler.PlaySe(_hitAudio);
            _hitTargets.Add(collider);
        }
    }
}