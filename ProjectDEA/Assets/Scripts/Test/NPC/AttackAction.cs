using System.Collections.Generic;
using Test.NPC.Dragon;
using UnityEngine;
using static Test.NPC.AnimatorControl;

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
        private readonly float _attackDuration;
        private readonly float _damage;
        private readonly float _stopFactor;

        private Transform _target;
        private readonly DebugDrawCd _debugDrawCd;
        private readonly HashSet<Collider> _hitTargets = new();
        private bool _isOnCooldown;
        private float _attackCt;
        private float _attackTimer;
        private bool _isOnAnim;

        private const float DamageCheckTime = 0.5f;
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
            _attackDuration = attackParameters._attackDuration;
            _damage = attackParameters._attackDamage;
            _stopFactor = attackParameters._stopFactor;
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
            _attackCt = 0;
            _attackTimer = DamageCheckTime;
            _movementControl.ChangeMove(true);
            _isOnAnim = false;
        }

        public void Execute(GameObject agent)
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
                _animatorControl.ChangeAnimBool(AnimationBool.Moving);
                return;
            }
          
            ExecuteDamageLoop();
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

            _attackCt -= Time.deltaTime;
            if (_attackCt <= 0)
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
            return FindTarget(_agent.position + _agent.forward * (_attackOffSetFactor * _stopFactor), _attackRadius) != null;
        }


        private void ExecuteDamageLoop()
        {
            if (!_isOnAnim)
            {
                _movementControl.ChangeMove(false);
                _animatorControl.OnTriggerAnim(AnimationTrigger.Attack);
                _isOnAnim = true;
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
            _isOnAnim = false;
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