using System;
using Item;
using Manager;
using UnityEngine;

namespace Gimmick
{
    public class KeyOut : MonoBehaviour, IInteractable, IGimmickID
    {
        private const ItemKind OutItem = ItemKind.Key;
        private InventoryHandler _inventoryHandler;
        public event Action Destroyed;
        public bool IsInteractable { get; private set; }
        public GimmickID GimmickIdInfo { get; set; }
        public event Action<IGimmickID> Returned;
        
        private void Start()
        {
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            IsInteractable = true;
        }

        public void Interact()
        {
            _inventoryHandler.AddItem(OutItem);
            Destroy(gameObject);
            Destroyed?.Invoke();
            Returned?.Invoke(this);
        }
    }
}
