using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Character.NPC.EnemyDragon
{
    public class DragonController : MonoBehaviour
    {
        private NavMeshAgent _agent;
        [SerializeField] private List<AIState> _drawableState;
        private AIState _currentState;
        private readonly Dictionary<AIState, INpcAiState> _states = new();
        
        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _states.Add(AIState.Stay, new StayState(gameObject));
            _states.Add(AIState.Attack, new AttackState());
            _states.Add(AIState.FreeWalk, new FreeWalkState(gameObject, _agent));

            _currentState = AIState.FreeWalk;
            _states[_currentState].EnterState();
        }

        private void Update()
        {
            _states[_currentState].UpdateState();
            if (_states[_currentState].IsStateFin) NextState();
        }

        private void NextState()
        {
            var newState = SelectState();
            _states[_currentState].ExitState();
            _states[newState].EnterState();
            _currentState = newState;
        }

        private AIState SelectState()
        {
            var drawableState = _drawableState;
            drawableState.Remove(_currentState);
            var rnd = Random.Range(0, drawableState.Count);
            return drawableState[rnd];
        }
        
    }
}
