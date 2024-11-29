using System;
using UnityEngine;
using Item;
using Manager;
using Manager.Map;
using Manager.MetaAI;
using Random = UnityEngine.Random;

namespace Gimmick
{
    public class TreasureBox : MonoBehaviour, IInteractable, IGimmickID
    {
        [SerializeField]
        private ItemKind[] _containItem;
        private ItemKind _outItem;
        private bool _isOpen;
        private InventoryHandler _inventoryHandler;
        public event Action Destroyed;

        [SerializeField] private GameObject _boxTop;
        [SerializeField] private Collider _boxTopCol;
        [SerializeField] private float _openDuration;
        [SerializeField] private Vector3 _targetRot;
        public bool IsInteractable { get; private set; }
        private MetaAIHandler _metaAIHandler;
        [SerializeField] private MetaAIHandler.AddScores[] _pickedScores;
        public GimmickID GimmickIdInfo { get; set; }
        public event Action<IGimmickID> Returned;
        private PlayerRoomTracker _playerRoomTracker;
        
        private void Start()
        {
            var number = Random.Range(0, _containItem.Length);
            _outItem = _containItem[number];
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            _playerRoomTracker = GameObject.FindWithTag("PlayerRoomTracker").GetComponent<PlayerRoomTracker>();
            _playerRoomTracker.OnPlayerRoomChange += OnTrashTreasure;
            IsInteractable = true;
            _metaAIHandler = GameObject.FindWithTag("MetaAI").GetComponent<MetaAIHandler>();
        }

        private void OnDestroy()
        {
            _playerRoomTracker.OnPlayerRoomChange -= OnTrashTreasure;
        }

        public void Interact()
        {
            if (_isOpen) return;
            IsInteractable = false;
            _inventoryHandler.AddItem(_outItem);
            _metaAIHandler.SendLogsForMetaAI(_pickedScores);
            _isOpen = true;
            StartCoroutine(RotateBoxTop());
        }
        
        private System.Collections.IEnumerator RotateBoxTop()
        {
            _boxTopCol.enabled = true;
            var startRotation = _boxTop.transform.localRotation;
            var endRotation = Quaternion.Euler(_targetRot);

            var elapsedTime = 0f;
            
            while (elapsedTime < _openDuration)
            {
                _boxTop.transform.localRotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / _openDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            _boxTop.transform.localRotation = endRotation;
        }

        private void OnTrashTreasure()
        {
            if (!_isOpen) return;
            Destroy(gameObject);
            Returned?.Invoke(this);
        }
    }
}