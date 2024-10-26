using System;
using Character.Player;
using Manager.Audio;
using UnityEngine;

namespace UI
{
    public class PanelSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject _inventoryPanel;
        [SerializeField] private PlayerClasHub _playerClasHub;
        public Action IsOpenInventory;
        private bool _isManipulate = true;
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _pushAudio;
        
        private void Start()
        {
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
        }

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
            _soundHandler.PlaySe(_pushAudio);
        }

        public void ChangeIsManipulate(bool isManipulate)
        {
            _isManipulate = isManipulate;
        }
    }
}
