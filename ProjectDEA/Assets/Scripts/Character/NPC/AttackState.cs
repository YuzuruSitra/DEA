using Character.NPC.EnemyDragon;
using UnityEngine;
using UnityEngine.AI;

namespace Character.NPC
{
    public class AttackState : INpcAiState
    {
        public DragonAnimCtrl.AnimState CurrentAnim => DragonAnimCtrl.AnimState.Idole;
        private readonly Transform _npcTransform;
        private readonly NavMeshAgent _agent;
        private readonly float _speed;
        private readonly float _currentAcceleration;
        private readonly float _acceleration;
        private readonly float _rotationSpeed;
        private Vector3 _targetPos;
        private bool _isRotating;
        public bool IsStateFin => IsStateFinJudgment();

        public AttackState(GameObject npc, NavMeshAgent agent, float speed, float acceleration, float rotationSpeed)
        {
            _npcTransform = npc.transform;
            _agent = agent;
            _speed = speed;
            _currentAcceleration = agent.acceleration;
            _acceleration = acceleration;
            _rotationSpeed = rotationSpeed;
            _isRotating = true;
        }

        public void EnterState()
        {
            SetTargetPosition();
            _agent.acceleration = _acceleration;
            _agent.isStopped = true;
            _isRotating = true;
        }

        public void UpdateState()
        {
            if (_isRotating)
            {
                RotateTowardsTarget();
                return;
            }
            
            _agent.isStopped = false;
            _agent.speed = _speed;
            _agent.SetDestination(_targetPos);
        }

        public void ExitState()
        {
            _agent.isStopped = true;
            _agent.acceleration = _currentAcceleration;
        }

        private void RotateTowardsTarget()
        {
            // ターゲットへの方向を計算
            var directionToTarget = (_targetPos - _npcTransform.position).normalized;
            var targetRotation = Quaternion.LookRotation(directionToTarget);

            // 現在の回転をターゲット方向に徐々に近づける
            _npcTransform.rotation = Quaternion.Slerp(_npcTransform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            // 回転がほぼ完了したら移動に切り替える
            if (Quaternion.Angle(_npcTransform.rotation, targetRotation) < 15f)
            {
                _isRotating = false;
            }
        }

        private bool IsStateFinJudgment()
        {
            return _agent.remainingDistance <= _agent.stoppingDistance;
        }

        private void SetTargetPosition()
        {
            var destinationDirection = (_agent.destination - _npcTransform.position).normalized;
            
            var layerMask = ~(LayerMask.GetMask("Water") | LayerMask.GetMask("Player"));
            
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
