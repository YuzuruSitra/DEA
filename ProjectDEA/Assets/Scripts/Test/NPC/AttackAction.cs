using AnimationState = Test.NPC.AnimatorControl.AnimationState;
using System.Collections.Generic;
using Test.NPC.Dragon;
using UnityEngine;

namespace Test.NPC
{
    public class AttackAction : IUtilityAction
    {
        public UtilityActionType ActionType => UtilityActionType.Attack;

        // Configuration
        private readonly AnimatorControl _animatorControl;
        private readonly MovementControl _movementControl;
        private readonly Transform _agent;
        private readonly Vector3 _searchOffSet;
        private readonly float _searchRadius;
        private readonly LayerMask _searchLayer;
        private readonly float _attackRadius;
        private readonly float _attackDelay;
        private readonly float _damage;
        private readonly Vector3 _attackOrigin;

        // State
        private Transform _target;
        private readonly DebugDrawCd _debugDrawCd;
        private bool _isOnCooldown;
        private float _cooldownTimer;
        private readonly HashSet<Collider> _hitTargets = new();
        private float _attackTimer;

        // Constants
        private const float AttackDuration = 2f;

        public AttackAction(Transform agent, AnimatorControl animatorControl, MovementControl movementControl, DragonController.AttackParameters attackParameters)
        {
            _agent = agent;
            _animatorControl = animatorControl;
            _movementControl = movementControl;
            _searchOffSet = attackParameters._searchOffSet;
            _searchRadius = attackParameters._searchRadius;
            _searchLayer = attackParameters._targetLayer;
            _attackRadius = attackParameters._attackRadius;
            _attackDelay = attackParameters._attackDelay;
            _damage = attackParameters._attackDamage;
            _attackOrigin = attackParameters._attackOffSet;
            _debugDrawCd = new DebugDrawCd();
        }

        public float CalculateUtility()
        {
            var center = _agent.position + _searchOffSet;
            var results = new Collider[1];
            var count = Physics.OverlapSphereNonAlloc(center, _searchRadius, results, _searchLayer, QueryTriggerInteraction.Ignore);
            if (count <= 0) return 0f;

            Debug.Log(results[0].gameObject.name);
            _target = results[0].transform;
            return 1f;
        }

        public void EnterState()
        {
            
        }

        public void Execute(GameObject agent)
        {
            DrawDebugSpheres(agent);

            // 攻撃のCT待機
            if (HandleCooldown()) return;

            // 対象の元へ移動
            if (!IsTargetValid())
            {
                SearchForTarget();
                return;
            }

            if (!IsTargetInRange())
            {
                _movementControl.MoveTo(_target.position);
                _animatorControl.SetAnimParameter(AnimationState.Moving);
                return;
            }

            _movementControl.Stop();

            // 対象を攻撃
            if (_attackTimer > 0)
            {
                UpdateAttack(agent);
                return;
            }

            StartAttack(agent);

            // 逃避
            // (追加の逃避ロジックが必要であればここに記述します)
        }

        private void DrawDebugSpheres(GameObject agent)
        {
            _debugDrawCd.DebugDrawSphere(_agent.position + _searchOffSet, _searchRadius, Color.blue);
            _debugDrawCd.DebugDrawSphere(agent.transform.position + _attackOrigin, _attackRadius, Color.red);
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

        private bool IsTargetValid()
        {
            return _target != null && Vector3.Distance(_agent.position, _target.position) <= _searchRadius;
        }

        private bool IsTargetInRange()
        {
            return Vector3.Distance(_agent.position, _target.position) <= _attackRadius;
        }

        private void SearchForTarget()
        {
            var center = _agent.position + _searchOffSet;
            var results = new Collider[1];
            var count = Physics.OverlapSphereNonAlloc(center, _searchRadius, results, _searchLayer, QueryTriggerInteraction.Ignore);
            if (count <= 0) return;
            _target = results[0].transform;
            Debug.Log($"Target acquired: {_target.name}");
        }

        private void StartAttack(GameObject agent)
        {
            _isOnCooldown = true;
            _cooldownTimer = _attackDelay;
            _attackTimer = AttackDuration;

            _animatorControl.SetAnimParameter(AnimationState.Attack);
            _hitTargets.Clear();
        }

        private void UpdateAttack(GameObject agent)
        {
            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0) return;

            ApplyDamageToTargets(agent);
        }

        private void ApplyDamageToTargets(GameObject agent)
        {
            var results = new Collider[5];
            var size = Physics.OverlapSphereNonAlloc(agent.transform.position + _attackOrigin, _attackRadius, results, _searchLayer, QueryTriggerInteraction.Ignore);

            for (var i = 0; i < size; i++)
            {
                var collider = results[i];
                if (_hitTargets.Contains(collider)) continue;

                var targetHealth = collider.GetComponent<HealthComponent>();
                if (targetHealth == null) continue;

                targetHealth.TakeDamage(_damage);
                Debug.Log($"Hit {collider.name} for {_damage} damage.");
                _hitTargets.Add(collider);
            }
        }
    }
}
