using System.Collections.Generic;
using Character.NPC.State;
using Character.Player;
using Manager.Audio;
using Test;
using UnityEngine;

namespace Character.NPC.Enemy.Golem
{
    public class GolemAttack1 : IBattleSubState
    {
        private readonly EnemyAnimHandler _enemyAnimHandler;
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
        private readonly float _searchUpPadding;
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

        public GolemAttack1(Transform agent, EnemyAnimHandler enemyAnimHandler, MovementControl movementControl, SoundHandler soundHandler, GolemController.ParamAttack1 paramAttack1)
        {
            _agent = agent;
            _enemyAnimHandler = enemyAnimHandler;
            _movementControl = movementControl;
            _soundHandler = soundHandler;
            _searchOffSetFactor = paramAttack1._searchOffSetFactor;
            _searchRadius = paramAttack1._searchRadius;
            _searchLayer = paramAttack1._targetLayer;
            _attackOffSetFactor = paramAttack1._attackOffSetFactor;
            _attackRadius = paramAttack1._attackRadius;
            _takeDamageWait = paramAttack1._takeDamageWait;
            _attackDuration = paramAttack1._attackDuration;
            _attackDamage = paramAttack1._attackDamage;
            _pushPower = paramAttack1._pushPower;
            _stopFactor = paramAttack1._stopFactor;
            _searchUpPadding = paramAttack1._searchUpPadding;
            _hitAudio = paramAttack1._hitAudio;
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
            _enemyAnimHandler.ChangeAnimBool(EnemyAnimHandler.AnimationBool.Moving);
            _isAttacking = false;
            _remainTakeDamageWait = _takeDamageWait;
        }

        public void Execute()
        {
            DrawDebugSpheres();
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
                _enemyAnimHandler.ChangeAnimBool(EnemyAnimHandler.AnimationBool.Moving);
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
            agentPos.y += _searchUpPadding;
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
            origin.y += _searchUpPadding;
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
                _enemyAnimHandler.OnTriggerAnim(EnemyAnimHandler.AnimationTrigger.Attack1);
                _isAttacking = true;
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
            _enemyAnimHandler.ChangeAnimBool(EnemyAnimHandler.AnimationBool.Moving);
            _isAttacking = false;
            _remainTakeDamageWait = _takeDamageWait;
        }

        private void ApplyDamageToTargets()
        {
            var origin = _agent.position;
            origin.y += _searchUpPadding;
            var size = Physics.OverlapSphereNonAlloc(origin + _agent.forward * _attackOffSetFactor, _attackRadius, _attackResults, _searchLayer, QueryTriggerInteraction.Ignore);
            if (size == 0) return;
            var collider = _attackResults[0];
            if (_hitTargets.Contains(collider)) return;
            if (!collider.TryGetComponent(out PlayerClasHub playerClasHub)) return;
            if (!playerClasHub.PlayerHpHandler.IsAddDamage) return;
            playerClasHub.PlayerMover.LaunchPushForceMove(_agent.forward.normalized, _pushPower);
            playerClasHub.PlayerHpHandler.ReceiveDamage(_attackDamage);
            _soundHandler.PlaySe(_hitAudio);
            _hitTargets.Add(collider);
        }
    }
}
