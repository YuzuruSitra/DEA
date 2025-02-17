using System;
using Character.Player;
using UnityEngine;

namespace Gimmick
{
    public class DamageArea : MonoBehaviour, IGimmickID
    {
        public GimmickID GimmickIdInfo { get; set; }
        public event Action<IGimmickID> Returned;
        
        private PlayerHpHandler _playerHpHandler;
        [SerializeField] private int _damageForSeconds;
        private float _currentTime;

        private void Start()
        {
            _currentTime = _damageForSeconds;
            _playerHpHandler = GameObject.FindWithTag("Player").GetComponent<PlayerHpHandler>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            _currentTime -= Time.deltaTime;
            if (_currentTime > 0) return;
            _playerHpHandler.ReceiveDamage(_damageForSeconds);
            _currentTime = 1;   
        }
    }
}
