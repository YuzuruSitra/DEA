using System;
using System.Collections;
using System.Collections.Generic;
using Character.Player;
using Item;
using Manager;
using Manager.Audio;
using Manager.PlayData;
using UI;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Character.NPC.EnemyDragon
{
    public class DragonController : MonoBehaviour
    {
        [SerializeField] private int _maxHp;
        public int MaxHp => _maxHp;
        private int _currentHp;
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
        public Action<int> ReceiveNewHp;
        public Action OnReviving;
        public bool IsDeath { get; private set; }
        private InventoryHandler _inventoryHandler;
        [SerializeField] private ItemKind[] _outItem;
        private LogTextHandler _logTextHandler;
        private const string SendLogMessage = "なんとかドラゴンを撃退した。";
        private AnalysisDataHandler _analysisDataHandler;
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _hitAudio;
        [SerializeField] private AudioClip _screamAudio;
        private WaitForSeconds _screamWait;
        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _counterWaitForSeconds = new WaitForSeconds(_counterWaitTime);
            _deathWaitForSeconds = new WaitForSeconds(_deathWait);
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            _logTextHandler = GameObject.FindWithTag("LogTextHandler").GetComponent<LogTextHandler>();
            _analysisDataHandler = GameObject.FindWithTag("AnalysisDataHandler").GetComponent<AnalysisDataHandler>();
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
            _screamWait = new WaitForSeconds(0.2f);
            
            _states.Add(AIState.Null, null);
            _states.Add(AIState.Stay, new StayState(gameObject, _stateTimeRange, _stayTimeBase));
            _states.Add(AIState.Attack, new AttackState(gameObject, _agent, _screamTime, _attackSpeed, _attackAcceleration, _attackRotSpeed));
            _states.Add(AIState.FreeWalk, new FreeWalkState(gameObject, _agent, _stateTimeRange, _walkTimeBase, _speed));
            CurrentState = AIState.Stay;
            _states[CurrentState].EnterState();
        }

        private void OnEnable()
        {
            OnInitialSetUp();
        }

        private void OnInitialSetUp()
        {
            IsDeath = false;
            _currentHp = _maxHp;
            if (_states.Count == 0) return;
            CurrentState = AIState.Stay;
            _states[CurrentState].EnterState();
            OnReviving?.Invoke();
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
            if (IsDeath) return;
            if (CurrentState == AIState.Attack) return;
            _agent.destination = target;
            NextState(AIState.Attack);
            StartCoroutine(ScreamWait());
        }

        private IEnumerator ScreamWait()
        {
            yield return _screamWait;
            _soundHandler.PlaySe(_screamAudio);
        }

        public void OnGetDamage(int damage, Vector3 targetPos = default)
        {
            if (IsDeath) return;
            _currentHp = Math.Max(_currentHp - damage, 0);
            ReceiveNewHp?.Invoke(_currentHp);
            if (_currentHp == 0)
            {
                IsDeath = true;
                _agent.isStopped = true;
                _logTextHandler.AddLog(SendLogMessage);
                var item = _outItem[Random.Range(0, _outItem.Length)];
                _inventoryHandler.AddItem(item);
                StartCoroutine(DeathDisable());
                _analysisDataHandler.EnemyKillCount ++;
                return;
            }
            if (targetPos == default) return;
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
            if (_states.Count == 0) return;
            if (!_states[CurrentState].IsAttacking) return;

            if (!other.collider.CompareTag("Player")) return;
            
            // プレイヤーのCharacterControllerを取得
            var playerHub = other.collider.GetComponent<PlayerClasHub>();
            if (playerHub == null) return;
            StartCoroutine(playerHub.PlayerMover.PushMoveUp(UpperDuration, _launchPower));
            playerHub.PlayerHpHandler.ReceiveDamage(_giveDamage);
            _soundHandler.PlaySe(_hitAudio);
        }
    }
}
