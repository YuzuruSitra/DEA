using System;
using Character.NPC;
using Item;
using Manager;
using Manager.MetaAI;
using Mission;
using UnityEngine;

namespace Gimmick
{
    public class BornOut : MonoBehaviour, IInteractable, IGimmickID
    {
        public event Action Destroyed;
        public bool IsInteractable { get; private set; }
        [SerializeField] private int _gimmickID;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private float _flyForce;
        [SerializeField] private float _rndRotRange;

        private GameObject _currentEnemy;
        private NpcController _currentNpcController;
        [SerializeField] private int _hitEnemyDamage;
        private bool _isMoving;
        private bool _hitOneTime;
        private const float MovementThreshold = 0.1f;
        private MetaAIHandler _metaAIHandler;
        [SerializeField] private MetaAIHandler.AddScores[] _kickedScores;
        [SerializeField] private MetaAIHandler.AddScores[] _pickedScores;
        public GimmickID GimmickIdInfo { get; set; }
        public event Action<IGimmickID> Returned;
        private GameEventManager _gameEventManager;
        private InventoryHandler _inventoryHandler;
        
        private void Start()
        {
            IsInteractable = false;
            _metaAIHandler = GameObject.FindWithTag("MetaAI").GetComponent<MetaAIHandler>();
            _gameEventManager = GameObject.FindWithTag("GameEventManager").GetComponent<GameEventManager>();
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
        }
        
        private void Update()
        {
            _isMoving = _rb.velocity.magnitude > MovementThreshold;
            if (!_isMoving) _hitOneTime = false;
        }
        
        public void Interact()
        {
            if (!IsInteractable) return;
            _inventoryHandler.AddItem(ItemKind.Born);
            _gameEventManager.GimmickCompleted(_gimmickID);
            _metaAIHandler.SendLogsForMetaAI(_pickedScores);
            Destroy(gameObject);
            Destroyed?.Invoke();
            Returned?.Invoke(this);
        }
        
        public void FlyAwayBorn(Vector3 playerPos)
        {
            if (_rb.constraints != RigidbodyConstraints.None) _rb.constraints = RigidbodyConstraints.None;
            var direction = (transform.position - playerPos).normalized;
            direction.y = 0;
            direction.Normalize();

            var randomAngle = UnityEngine.Random.Range(-_rndRotRange, _rndRotRange);

            var rotation = Quaternion.Euler(0, randomAngle, 0);
            var randomizedDirection = rotation * direction;

            _rb.AddForce(randomizedDirection * _flyForce, ForceMode.Impulse);
            IsInteractable = true;
            _metaAIHandler.SendLogsForMetaAI(_kickedScores);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isMoving) return;
            if (_hitOneTime) return;
            if (!other.CompareTag("Enemy")) return;
            if (_currentEnemy != other.gameObject)
            {
                _currentEnemy = other.gameObject;
                _currentNpcController = _currentEnemy.GetComponent<NpcController>();
            }

            _currentNpcController.OnGetDamage(_hitEnemyDamage);
            _hitOneTime = true;
        }
    }
}
