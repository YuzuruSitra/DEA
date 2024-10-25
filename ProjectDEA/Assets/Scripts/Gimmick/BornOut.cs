using System;
using Character.NPC.EnemyDragon;
using Item;
using Manager;
using UnityEngine;

namespace Gimmick
{
    public class BornOut : MonoBehaviour, IInteractable
    {
        private const ItemKind OutItem = ItemKind.Born;
        private InventoryHandler _inventoryHandler;
        public event Action Destroyed;
        public bool IsInteractable { get; private set; }

        [SerializeField] private Rigidbody _rb;
        [SerializeField] private float _flyForce;
        [SerializeField] private float _rndRotRange;

        private GameObject _currentDragon;
        private DragonController _currentDragonController;
        [SerializeField] private int _hitEnemyDamage;
        private bool _isMoving;
        private bool _hitOneTime;
        private const float MovementThreshold = 0.1f;

        private void Start()
        {
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            IsInteractable = false;
        }

        private void Update()
        {
            _isMoving = _rb.velocity.magnitude > MovementThreshold;
            if (!_isMoving) _hitOneTime = false;
        }

        public void Interact()
        {
            if (!IsInteractable) return;
            _inventoryHandler.AddItem(OutItem);
            Destroy(gameObject);
            Destroyed?.Invoke();
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
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isMoving) return;
            if (_hitOneTime) return;
            if (!other.CompareTag("EnemyDragon")) return;
            if (_currentDragon != other.gameObject)
            {
                _currentDragon = other.gameObject;
                _currentDragonController = _currentDragon.GetComponent<DragonController>();
            }

            _currentDragonController.OnGetDamage(_hitEnemyDamage);
            _hitOneTime = true;
        }
    }
}
