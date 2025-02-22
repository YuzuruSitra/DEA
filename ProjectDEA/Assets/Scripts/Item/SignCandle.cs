using System;
using Gimmick;
using Manager;
using Manager.Audio;
using UnityEngine;

namespace Item
{
    public class SignCandle : MonoBehaviour, IInteractable
    {
        public event Action Destroyed;
        public bool IsInteractable { get; private set; }
        private InventoryHandler _inventoryHandler;
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _signCandleSound;

        private void Start()
        {
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
            IsInteractable = true;
            _soundHandler.PlaySe(_signCandleSound);
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
