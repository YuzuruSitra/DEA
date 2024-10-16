using UnityEngine;
using UnityEngine.AI;

namespace Character.NPC
{
    public class AttackState : INpcAiState
    {
        private readonly Transform _npcTransform;
        private readonly NavMeshAgent _agent;
        private readonly float _speed;
        private readonly float _rotationSpeed;
        private Vector3 _targetPos;
        private bool _isRotating;
        public bool IsStateFin => IsStateFinJudgment();

        public AttackState(GameObject npc, NavMeshAgent agent, float speed, float rotationSpeed)
        {
            _npcTransform = npc.transform;
            _agent = agent;
            _speed = speed;
            _rotationSpeed = rotationSpeed;
            _isRotating = true;
        }

        public void EnterState()
        {
            _targetPos = _agent.destination;
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
            
            // 回転が完了したらNavMeshAgentを使って突進
            _agent.isStopped = false;
            _agent.speed = _speed;
            _agent.SetDestination(_targetPos);
        }

        public void ExitState()
        {
            _agent.isStopped = true;
        }

        private void RotateTowardsTarget()
        {
            // ターゲットへの方向を計算
            var directionToTarget = (_targetPos - _npcTransform.position).normalized;
            var targetRotation = Quaternion.LookRotation(directionToTarget);

            // 現在の回転をターゲット方向に徐々に近づける
            _npcTransform.rotation = Quaternion.Slerp(_npcTransform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            // 回転がほぼ完了したら移動に切り替える
            if (Quaternion.Angle(_npcTransform.rotation, targetRotation) < 1f)
            {
                _isRotating = false;
            }
        }

        private bool HasReachedDestination()
        {
            // remainingDistanceが0.1f以下になったら目的地に到達したとみなす
            return !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance;
        }
        
        // 目的地に到達したか、進行できない場合は停止
        private bool IsStateFinJudgment()
        {
            return HasReachedDestination() || _agent.pathStatus != NavMeshPathStatus.PathComplete;
        }
    }
}
