using System;
using UnityEngine;
using Item;
using Manager;
using Random = UnityEngine.Random;

namespace Gimmick
{
    public class TreasureBox : MonoBehaviour, IInteractable
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
        
        private void Start()
        {
            var number = Random.Range(0, _containItem.Length);
            _outItem = _containItem[number];
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
        }

        public void Interact()
        {
            if (_isOpen) return;
            _inventoryHandler.AddItem(_outItem);
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
    }
}