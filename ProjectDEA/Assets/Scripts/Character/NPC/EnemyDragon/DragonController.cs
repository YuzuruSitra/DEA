using System;
using System.Collections;
using System.Collections.Generic;
using Character.Player;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Character.NPC.EnemyDragon
{
    public class DragonController : MonoBehaviour
    {
        [SerializeField] private int _currentHp;
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
        [SerializeField] private float _attackAcceleration;
        [SerializeField] private float _screamTime;
        [SerializeField] private float _launchPower;
        [SerializeField] private int _giveDamage;
        [SerializeField] private int _counterWaitTime;
        private WaitForSeconds _counterWaitForSeconds;
        [SerializeField] private float _deathWait;
        private WaitForSeconds _deathWaitForSeconds;
        private Coroutine _counterCoroutine;
        private const float UpperDuration = 0.5f;
        public DragonAnimCtrl.AnimState AnimState => _states[CurrentState].CurrentAnim;
        public Action GetDamage;
        public bool IsDeath { get; private set; }
        public Action DoDeath;
        
        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _counterWaitForSeconds = new WaitForSeconds(_counterWaitTime);
            _deathWaitForSeconds = new WaitForSeconds(_deathWait);
            _states.Add(AIState.Null, null);
            _states.Add(AIState.Stay, new StayState(gameObject, _stateTimeRange, _stayTimeBase));
            _states.Add(AIState.Attack, new AttackState(gameObject, _agent, _screamTime, _attackSpeed, _attackAcceleration, _attackRotSpeed));
            _states.Add(AIState.FreeWalk, new FreeWalkState(gameObject, _agent, _stateTimeRange, _walkTimeBase, _speed));
            CurrentState = AIState.Stay;
            _states[CurrentState].EnterState();
        }

        private void OnEnable()
        {
            IsDeath = false;
            _agent.isStopped = false;
            if (_states.Count == 0) return;
            CurrentState = AIState.Stay;
            _states[CurrentState].EnterState();
        }
        
        private void Update()
        {
            if (IsDeath) return;
            _states[CurrentState].UpdateState();
            if (_states[CurrentState].IsStateFin) NextState();
        }

        private void NextState(AIState state = AIState.Null)
        {
            var newState = state == AIState.Null ? SelectState() : state;
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

        public void OnGetDamage(int damage, Vector3 targetPos)
        {
            if (IsDeath) return;
            _currentHp = Math.Max(_currentHp - damage, 0);
            if (_currentHp == 0)
            {
                IsDeath = true;
                _agent.isStopped = true;
                DoDeath?.Invoke();
                StartCoroutine(DeathDisable());
                return;
            }
            GetDamage?.Invoke();
            _counterCoroutine ??= StartCoroutine(CounterAttackStateChange(targetPos));
        }

        private IEnumerator CounterAttackStateChange(Vector3 targetPos)
        {
            yield return _counterWaitForSeconds;
            if (CurrentState != AIState.Attack) OnAttackState(targetPos);
            _counterCoroutine = null;
        }

        private IEnumerator DeathDisable()
        {
            yield return _deathWaitForSeconds;
            gameObject.SetActive(false);
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (!_states[CurrentState].IsAttacking) return;

            if (!other.collider.CompareTag("Player")) return;
            
            // プレイヤーのCharacterControllerを取得
            var playerHub = other.collider.GetComponent<PlayerClasHub>();
            if (playerHub == null) return;
            StartCoroutine(playerHub.PlayerMover.PushMoveUp(UpperDuration, _launchPower));
            playerHub.PlayerHpHandler.ReceiveDamage(_giveDamage);
        }
    }
}
