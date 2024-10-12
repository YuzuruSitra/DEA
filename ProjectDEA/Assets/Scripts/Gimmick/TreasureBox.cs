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
        }
    }
}
