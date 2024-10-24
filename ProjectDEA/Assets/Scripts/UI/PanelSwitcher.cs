using System;
using Character.Player;
using UnityEngine;

namespace UI
{
    public class PanelSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject _inventoryPanel;
        [SerializeField] private PlayerClasHub _playerClasHub;
        public Action IsOpenInventory;
        private bool _isManipulate = true;

        private void Update()
        {
            if (!_isManipulate) return;
            if (Input.GetKeyDown(KeyCode.Tab)) ChangeInventoryPanel();
        }

        private void ChangeInventoryPanel()
        {
            var active = _inventoryPanel.activeSelf;
            _inventoryPanel.SetActive(!active);
            _playerClasHub.SetPlayerFreedom(active);
            IsOpenInventory?.Invoke();
        }

        public void ChangeIsManipulate(bool isManipulate)
        {
            _isManipulate = isManipulate;
        }
    }
}
