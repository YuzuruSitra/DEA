using UnityEngine;
using UnityEngine.AI;

namespace PartnerNPC
{
    public class FreeWalkState : IPartnerAIState
    {
        
        private readonly Transform _npcTransform;
        private readonly NavMeshAgent _agent;
        private readonly float _speed = 4.0f;
        private float _currentWait;
        private const float WaitTime = 1.0f;
        private const float Range = 5.0f;
        private const float AngleRange = 90f;
        private const float DestinationThreshold = 1.25f;
        private const float StateTime = 6.0f;
        private float _remainTime;
        public bool IsStateFin => (_remainTime <= 0);

        public FreeWalkState(GameObject npc, NavMeshAgent agent)
        {
            _npcTransform = npc.transform;
            _agent = agent;
        }
        
        public void EnterState()
        {
            _remainTime = StateTime;
            _agent.isStopped = false;
            SetRandomDestination();
            _agent.speed = _speed;
            _currentWait = WaitTime;
        }
        
        public void UpdateState()
        {
            _remainTime -= Time.deltaTime;
            if (!_agent.pathPending && _agent.remainingDistance <= DestinationThreshold)
                SetRandomDestination();
        }

        public void ExitState()
        {
            _agent.isStopped = true;
        }

        // Set a target point.
        private void SetRandomDestination()
        {
            _currentWait -= Time.deltaTime;
            if (_currentWait >= 0) return;
            var forward = _npcTransform.transform.forward;
            
            var randomAngle = Random.Range(-AngleRange, AngleRange);
            var direction = Quaternion.Euler(0, randomAngle, 0) * forward;
            
            var randomDistance = Random.Range(0, Range);
            var destination = _npcTransform.position + direction * randomDistance;
            
            if (NavMesh.SamplePosition(destination, out NavMeshHit hit, Range, NavMesh.AllAreas))
                _agent.SetDestination(hit.position);
            _currentWait = WaitTime;
        }

    }
}
