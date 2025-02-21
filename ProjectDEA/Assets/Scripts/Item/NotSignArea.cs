using Character.Player;
using UnityEngine;

namespace Item
{
    public class NotSignArea : MonoBehaviour
    {
        private PlayerUseItem _playerUseItem;

        private void OnDestroy()
        {
            if (_playerUseItem != null)
            {
                _playerUseItem.CanUseCandle = true;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (_playerUseItem == null) _playerUseItem = other.gameObject.GetComponent<PlayerUseItem>();
            _playerUseItem.CanUseCandle = false;
            Debug.Log(_playerUseItem.CanUseCandle);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (_playerUseItem == null) return;
            _playerUseItem.CanUseCandle = true;
        }
    }
}
