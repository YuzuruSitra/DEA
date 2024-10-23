using System;
using UnityEngine;

namespace UI
{
    public class PanelSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject _inventoryPanel;
        public Action IsOpenInventory;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab)) ChangeInventoryPanel();
        }

        private void ChangeInventoryPanel()
        {
            var active = _inventoryPanel.activeSelf;
            _inventoryPanel.SetActive(!active);
            IsOpenInventory?.Invoke();
        }
    }
}
