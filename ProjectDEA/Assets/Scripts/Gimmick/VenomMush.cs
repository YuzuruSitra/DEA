using System;
using Character.Player;
using Manager.Audio;
using UnityEngine;

namespace Gimmick
{
    public class VenomMush : MonoBehaviour, IGimmickID
    {
        private PlayerHpHandler _playerHpHandler;
        private SoundHandler _soundHandler;
        public GimmickID GimmickIdInfo { get; set; }
        public event Action<IGimmickID> Returned;
        [SerializeField] private int _damageForSeconds;
        private const float OneSecond = 1.0f;
        private float _currentTime = OneSecond;
        [SerializeField] private AudioClip _breakedSe;

        private void Start()
        {
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
        }
        
        public void OnDestroy()
        {
            _soundHandler.PlaySe(_breakedSe);
            Returned?.Invoke(this);
            Destroy(gameObject);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            if (_playerHpHandler == null)
            {
                _playerHpHandler = other.GetComponent<PlayerHpHandler>();
            }
            _currentTime -= Time.deltaTime;
            if (_currentTime > 0) return;
            _playerHpHandler.ReceiveDamage(_damageForSeconds);
            _currentTime = OneSecond;   
        }
        
    }
}
