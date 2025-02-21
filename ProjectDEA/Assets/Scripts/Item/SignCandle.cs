using System;
using Gimmick;
using Manager;
using UnityEngine;

namespace Item
{
    public class SignCandle : MonoBehaviour, IInteractable
    {
        public event Action Destroyed;
        public bool IsInteractable { get; private set; }
        private InventoryHandler _inventoryHandler;

        private void Start()
        {
            IsInteractable = true;
        }

        public void Interact()
        {
            if (!IsInteractable) return;
            _inventoryHandler.AddItem(ItemKind.SignCandle);
            Destroy(gameObject);
            Destroyed?.Invoke();
        }
    }
}
