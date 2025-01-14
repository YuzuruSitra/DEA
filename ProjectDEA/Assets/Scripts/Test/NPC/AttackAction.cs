using AnimationState = Test.NPC.AnimatorControl.AnimationState;
using System.Collections.Generic;
using Test.NPC.Dragon;
using UnityEngine;

namespace Test.NPC
{
    public class AttackAction : IUtilityAction
    {
        public UtilityActionType ActionType => UtilityActionType.Attack;

        private readonly AnimatorControl _animatorControl;
        private readonly MovementControl _movementControl;
        private readonly Transform _agent;

        private readonly float _searchOffSetFactor;
        private readonly float _searchRadius;
        private readonly LayerMask _searchLayer;
        private readonly float _attackOffSetFactor;
        private readonly float _attackRadius;
        private readonly float _attackDelay;
        private readonly float _damage;

        private Transform _target;
        private readonly DebugDrawCd _debugDrawCd;
        private readonly HashSet<Collider> _hitTargets = new();
        private bool _isOnCooldown;
        private float _cooldownTimer;
        private float _attackTimer;

        private const float AttackDuration = 2f;
        private const float StopFactor = 0.4f;
        private readonly Collider[] _searchResults = new Collider[1];
        private readonly Collider[] _attackResults = new Collider[1];

        public AttackAction(Transform agent, AnimatorControl animatorControl, MovementControl movementControl, DragonController.AttackParameters attackParameters)
        {
            _agent = agent;
            _animatorControl = animatorControl;
            _movementControl = movementControl;
            _searchOffSetFactor = attackParameters._searchOffSetFactor;
            _searchRadius = attackParameters._searchRadius;
            _searchLayer = attackParameters._targetLayer;
            _attackOffSetFactor = attackParameters._attackOffSetFactor;
            _attackRadius = attackParameters._attackRadius;
            _attackDelay = attackParameters._attackDelay;
            _damage = attackParameters._attackDamage;
            _debugDrawCd = new DebugDrawCd();
        }

        public float CalculateUtility()
        {
            _target = FindTarget(_agent.position + _agent.forward * _searchOffSetFactor, _searchRadius);
            return _target != null ? 1f : 0f;
        }

        public void EnterState()
        {
            _isOnCooldown = false;
            _cooldownTimer = 0;
            _attackTimer = 0;
        }

        public void Execute(GameObject agent)
        {
            DrawDebugSpheres();

            if (HandleCooldown())
            {
                _animatorControl.SetAnimParameter(AnimationState.Moving);
                return;
            }

            if (_target == null || !IsTargetValid())
            {
                _target = FindTarget(_agent.position + _agent.forward * _searchOffSetFactor, _searchRadius);
                return;
            }

            if (!IsTargetInRange())
            {
                _movementControl.ChangeMove(true);
                _movementControl.MoveTo(_target.position);
                _animatorControl.SetAnimParameter(AnimationState.Moving);
                return;
            }
            _movementControl.ChangeMove(false);

            PerformAttack();
        }

        public void ExitState()
        {
            _movementControl.ChangeMove(true);
        }

        private void DrawDebugSpheres()
        {
            _debugDrawCd.DebugDrawSphere(_agent.position + _agent.forward * _searchOffSetFactor, _searchRadius, Color.blue);
            _debugDrawCd.DebugDrawSphere(_agent.position + _agent.forward * _attackOffSetFactor, _attackRadius, Color.red);
        }

        private bool HandleCooldown()
        {
            if (!_isOnCooldown) return false;

            _cooldownTimer -= Time.deltaTime;
            if (_cooldownTimer <= 0)
            {
                _isOnCooldown = false;
            }
            return true;
        }

        private Transform FindTarget(Vector3 origin, float radius)
        {
            var count = Physics.OverlapSphereNonAlloc(origin, radius, _searchResults, _searchLayer, QueryTriggerInteraction.Ignore);
            return (count > 0 && _searchResults[0] != null) ? _searchResults[0].transform : null;
        }

        private bool IsTargetValid()
        {
            return FindTarget(_agent.position + _agent.forward * _searchOffSetFactor, _searchRadius) != null;
        }

        private bool IsTargetInRange()
        {
            return FindTarget(_agent.position + _agent.forward * (_attackOffSetFactor * StopFactor), _attackRadius) != null;
        }

        private void PerformAttack()
        {
            if (_attackTimer > 0)
            {
                UpdateAttack();
                return;
            }
            StartAttack();
        }

        private void StartAttack()
        {
            _isOnCooldown = true;
            _cooldownTimer = _attackDelay;
            _attackTimer = AttackDuration;

            _animatorControl.SetAnimParameter(AnimationState.Attack);
            _hitTargets.Clear();
        }

        private void UpdateAttack()
        {
            _attackTimer -= Time.deltaTime;
            if (_attackTimer > 0)
            {
                ApplyDamageToTargets();
            }
        }

        private void ApplyDamageToTargets()
        {
            var size = Physics.OverlapSphereNonAlloc(_agent.position + _agent.forward * _attackOffSetFactor, _attackRadius, _attackResults, _searchLayer, QueryTriggerInteraction.Ignore);
            if (size == 0) return;
            var collider = _attackResults[0];
            if (_hitTargets.Contains(collider)) return;
            if (!collider.TryGetComponent(out HealthComponent targetHealth)) return;
            targetHealth.TakeDamage(_damage);
            _hitTargets.Add(collider);
        }
    }
}