using System;
using Item;
using Manager;
using UnityEngine;

namespace Gimmick
{
    public class BornOut : MonoBehaviour,IInteractable
    {
        private const ItemKind OutItem = ItemKind.Born;
        private InventoryHandler _inventoryHandler;
        public event Action Destroyed;
        private void Start()
        {
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
        }

        public void Interact()
        {
            _inventoryHandler.AddItem(OutItem);
            Destroy(gameObject);
            Destroyed?.Invoke();
        }
    }
}
