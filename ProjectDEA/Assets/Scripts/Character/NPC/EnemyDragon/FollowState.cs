using UnityEngine;
using UnityEngine.AI;

namespace Character.NPC.EnemyDragon
{
    public class FollowState : INpcAiState
    {
        public bool IsAttacking { get; }
        public DragonAnimCtrl.AnimState CurrentAnim { get; private set; }
        private readonly Transform _player;
        private readonly NavMeshAgent _agent;
        private const float Speed = 8.0f;
        public bool IsStateFin => false;

        public FollowState(GameObject player, NavMeshAgent agent)
        {
            _player = player.transform;
            _agent = agent;
            IsAttacking = false;
        }
        
        public void EnterState()
        {
            _agent.isStopped = false;
            _agent.speed = Speed;
        }
        
        public void UpdateState()
        {
            _agent.destination = _player.position;
        }

        public void ExitState()
        {
            _agent.isStopped = true;
        }
    }
}
