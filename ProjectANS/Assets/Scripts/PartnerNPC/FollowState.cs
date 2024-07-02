using UnityEngine;
using UnityEngine.AI;

namespace PartnerNPC
{
    public class FollowState : IPartnerAIState
    {
        private readonly Transform _player;
        private readonly NavMeshAgent _agent;
        private const float Speed = 6.0f;
        private const float StateTime = 4.0f;
        private float _remainTime;
        public bool IsStateFin => (_remainTime <= 0);

        public FollowState(GameObject player, NavMeshAgent agent)
        {
            _player = player.transform;
            _agent = agent;
        }
        
        public void EnterState()
        {
            _remainTime = StateTime;
            _agent.isStopped = false;
            _agent.speed = Speed;
        }
        
        public void UpdateState()
        {
            _agent.destination = _player.position;
            _remainTime -= Time.deltaTime;
        }

        public void ExitState()
        {
            _agent.isStopped = true;
        }
    }
}
