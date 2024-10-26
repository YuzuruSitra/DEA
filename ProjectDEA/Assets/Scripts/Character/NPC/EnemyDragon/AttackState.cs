using System;
using UnityEngine;
using UnityEngine.AI;

namespace Character.NPC.EnemyDragon
{
    public class AttackState : INpcAiState
    {
        public DragonAnimCtrl.AnimState CurrentAnim { get; private set; }
        public bool IsAttacking { get; private set; }
        private readonly Transform _npcTransform;
        private readonly NavMeshAgent _agent;
        private readonly float _speed;
        private readonly float _currentAcceleration;
        private readonly float _acceleration;
        private readonly float _rotationSpeed;
        private Vector3 _targetPos;
        public bool IsStateFin => IsStateFinJudgment();
        private readonly float _screamTime;
        private float _remainScreamTime;

        private enum State
        {
            Rotating,
            Screaming,
            Run
        }

        private State _currentState;

        public AttackState(GameObject npc, NavMeshAgent agent, float screamTime, float speed, float acceleration, float rotationSpeed)
        {
            _npcTransform = npc.transform;
            _agent = agent;
            _screamTime = screamTime;
            _speed = speed;
            _currentAcceleration = agent.acceleration;
            _acceleration = acceleration;
            _rotationSpeed = rotationSpeed;
        }

        public void EnterState()
        {
            SetTargetPosition();
            _agent.acceleration = _acceleration;
            _remainScreamTime = _screamTime;
            _agent.isStopped = true;
            _currentState = State.Rotating;
        }

        public void UpdateState()
        {
            
            switch(_currentState)
            {
                case State.Rotating:
                    RotateTowardsTarget();
                    break;
                case State.Screaming:
                    CurrentAnim = DragonAnimCtrl.AnimState.IsScream;
                    _remainScreamTime -= Time.deltaTime;
                    if (_remainScreamTime <= 0) _currentState = State.Run;
                    break;
                case State.Run:
                    CurrentAnim = DragonAnimCtrl.AnimState.IsRun;
                    IsAttacking = true;
                    _agent.isStopped = false;
                    _agent.speed = _speed;
                    _agent.SetDestination(_targetPos);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ExitState()
        {
            IsAttacking = false;
            _agent.isStopped = true;
            _agent.acceleration = _currentAcceleration;
        }

        private void RotateTowardsTarget()
        {
            CurrentAnim = DragonAnimCtrl.AnimState.Idole;
            // ターゲットへの方向を計算
            var directionToTarget = (_targetPos - _npcTransform.position).normalized;
            var targetRotation = Quaternion.LookRotation(directionToTarget);

            // 現在の回転をターゲット方向に徐々に近づける
            _npcTransform.rotation = Quaternion.Slerp(_npcTransform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            // 回転がほぼ完了したら移動に切り替える
            if (Quaternion.Angle(_npcTransform.rotation, targetRotation) < 15f)
            {
                _currentState = State.Screaming;
            }
        }

        private bool IsStateFinJudgment()
        {
            return IsAttacking && _agent.remainingDistance <= _agent.stoppingDistance;
        }

        private void SetTargetPosition()
        {
            // 正面方向のy成分を無視してXZ平面での方向を取得
            var destinationDirection = (_agent.destination - _npcTransform.position).normalized;
            destinationDirection.y = 0;  // y成分をゼロにして高さを無視

            var layerMask = ~(LayerMask.GetMask("Water") | LayerMask.GetMask("Player") | LayerMask.GetMask("Other"));

            const float maxDistance = 100f;
            if (Physics.Raycast(_npcTransform.position, destinationDirection, out var hit, maxDistance, layerMask))
                _targetPos = hit.point;
            else
                _targetPos = _npcTransform.position + destinationDirection * maxDistance;

            if (!NavMesh.SamplePosition(_targetPos, out var navMeshHit, maxDistance, NavMesh.AllAreas)) return;
            _targetPos = navMeshHit.position;
        }

    }
}
