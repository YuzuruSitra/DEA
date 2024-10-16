using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Character.NPC.EnemyDragon
{
    public class DragonController : MonoBehaviour
    {
        private NavMeshAgent _agent;
        [SerializeField] private List<AIState> _drawableState;
        public AIState CurrentState { get; private set; }
        private readonly Dictionary<AIState, INpcAiState> _states = new();
        [SerializeField] private float _stateTimeRange;
        [SerializeField] private float _stayTimeBase;
        [SerializeField] private float _walkTimeBase;
        [SerializeField] private float _speed;
        [SerializeField] private float _attackSpeed;
        [SerializeField] private float _attackRotSpeed;
        public DragonAnimCtrl.AnimState AnimState => _states[CurrentState].CurrentAnim;
        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _states.Add(AIState.Null, null);
            _states.Add(AIState.Stay, new StayState(gameObject, _stateTimeRange, _stayTimeBase));
            _states.Add(AIState.Attack, new AttackState(gameObject, _agent, _attackSpeed, _attackRotSpeed));
            _states.Add(AIState.FreeWalk, new FreeWalkState(gameObject, _agent, _stateTimeRange, _walkTimeBase, _speed));

            CurrentState = AIState.Stay;
            _states[CurrentState].EnterState();
        }

        private void Update()
        {
            _states[CurrentState].UpdateState();
            if (_states[CurrentState].IsStateFin) NextState();
        }

        private void NextState(AIState state = AIState.Null)
        {
            var newState = state == AIState.Null ? SelectState() : state;
            Debug.Log(newState);
            _states[CurrentState].ExitState();
            _states[newState].EnterState();
            CurrentState = newState;
        }

        private AIState SelectState()
        {
            var drawableState = new List<AIState>(_drawableState);
            drawableState.Remove(CurrentState);
            var rnd = Random.Range(0, drawableState.Count);
            return drawableState[rnd];
        }

        public void OnAttackState(Vector3 target)
        {
            _agent.destination = target;
            NextState(AIState.Attack);
        }
    }
}
