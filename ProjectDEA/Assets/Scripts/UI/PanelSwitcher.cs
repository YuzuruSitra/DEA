using System;
using Character.Player;
using Manager.Audio;
using UnityEngine;

namespace UI
{
    public class PanelSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject _inventoryPanel;
        [SerializeField] private GameObject _memoirsPanel;
        [SerializeField] private PlayerClasHub _playerClasHub;
        public Action IsOpenInventory;
        private bool _isManipulate = true;
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _pushAudio;
        private InputActions _inputActions;
        
        private void Start()
        {
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
            // Interactアクションのイベントリスナーを登録
            _inputActions = new InputActions();
            _inputActions.Player.ChangeInventryPanel.performed += ChangeInventoryPanel;
            _inputActions.Player.CloseMemoirsPanel.performed += CloseMemoirsPanel;
            _inputActions.Enable();
        }

        private void OnDestroy()
        {
            _inputActions.Player.ChangeInventryPanel.performed -= ChangeInventoryPanel;
            _inputActions.Player.CloseMemoirsPanel.performed -= CloseMemoirsPanel;
            _inputActions.Disable();
        }

        private void ChangeInventoryPanel(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (!_isManipulate) return;
            if (_memoirsPanel.activeSelf) return;
            var active = _inventoryPanel.activeSelf;
            _inventoryPanel.SetActive(!active);
            _playerClasHub.SetPlayerFreedom(active);
            IsOpenInventory?.Invoke();
            _soundHandler.PlaySe(_pushAudio);
        }

        private void CloseMemoirsPanel(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (!_memoirsPanel.activeSelf) return;
            ChangeMemoirsPanel(false);
        }
        
        public void ChangeMemoirsPanel(bool isOpen)
        {
            if (!_isManipulate) return;
            _memoirsPanel.SetActive(isOpen);
            _playerClasHub.SetPlayerFreedom(!isOpen);
            _soundHandler.PlaySe(_pushAudio);
        }

        public void ChangeIsManipulate(bool isManipulate)
        {
            _isManipulate = isManipulate;
        }
    }
}
