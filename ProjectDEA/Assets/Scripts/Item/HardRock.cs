using System;
using Character.NPC;
using Gimmick;
using Manager;
using Manager.MetaAI;
using Mission;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Item
{
    public class HardRock : MonoBehaviour, IInteractable
    {
        public event Action Destroyed;
        public bool IsInteractable { get; private set; }
        [SerializeField] private int _missionItemID;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private float _flyForce;
        [SerializeField] private float _rndRotRange;
        [SerializeField] private int _hitEnemyDamage;
        [SerializeField] private MetaAIHandler.AddScores[] _kickedScores;
        
        private MetaAIHandler _metaAIHandler;
        private GameEventManager _gameEventManager;
        private InventoryHandler _inventoryHandler;
        private bool _isMoving;
        private bool _hitOneTime;
        private const float MovementThreshold = 0.1f;
        private GameObject _currentEnemy;
        private NpcController _currentNpcController;
        
        private void Start()
        {
            _metaAIHandler = GameObject.FindWithTag("MetaAI").GetComponent<MetaAIHandler>();
            _gameEventManager = GameObject.FindWithTag("GameEventManager").GetComponent<GameEventManager>();
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            IsInteractable = true;
        }
        
        private void Update()
        {
            _isMoving = _rb.velocity.magnitude > MovementThreshold;
            if (!_isMoving) _hitOneTime = false;
        }
        
        public void Interact()
        {
            if (!IsInteractable) return;
            _inventoryHandler.AddItem(ItemKind.HardStone);
            Destroy(gameObject);
            Destroyed?.Invoke();
        }
        
        
        public void FlyAwayStone(Vector3 playerPos)
        {
            if (_rb.constraints != RigidbodyConstraints.None) _rb.constraints = RigidbodyConstraints.None;
            var direction = (transform.position - playerPos).normalized;
            direction.y = 0;
            direction.Normalize();

            var randomAngle = Random.Range(-_rndRotRange, _rndRotRange);

            var rotation = Quaternion.Euler(0, randomAngle, 0);
            var randomizedDirection = rotation * direction;

            _rb.AddForce(randomizedDirection * _flyForce, ForceMode.Impulse);
            _metaAIHandler.SendLogsForMetaAI(_kickedScores);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_isMoving) return;
            if (_hitOneTime) return;
            if (!collision.collider.CompareTag("Enemy")) return;
            if (_currentEnemy != collision.gameObject)
            {
                _currentEnemy = collision.gameObject;
                _currentNpcController = _currentEnemy.GetComponent<NpcController>();
            }

            _currentNpcController.OnGetDamage(_hitEnemyDamage);
            _hitOneTime = true;
            _gameEventManager.ItemUsed(_missionItemID);
        }
    }
}
