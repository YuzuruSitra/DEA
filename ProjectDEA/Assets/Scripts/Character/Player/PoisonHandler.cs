using UnityEngine;

namespace Character.Player
{
    public class PoisonHandler : MonoBehaviour
    {
        [SerializeField] private PlayerHpHandler _playerHpHandler;
        [SerializeField] private float _damageForSeconds;
        private float _currentTime;
        private bool _isAddDamage = true;
        private void Start()
        {
            _currentTime = _damageForSeconds;
        }

        private void Update()
        {
            if (!_isAddDamage) return;
            _currentTime -= Time.deltaTime;
            if (_currentTime > 0) return;
            _playerHpHandler.ReceiveDamage(1);
            _currentTime = _damageForSeconds;
        }

        public void ChangeIsAddDamage(bool state)
        {
            _isAddDamage = state;
        }
        
    }
}
