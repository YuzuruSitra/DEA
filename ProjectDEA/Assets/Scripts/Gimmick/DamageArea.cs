using System;
using Character.Player;
using Manager.MetaAI;
using UnityEngine;

namespace Gimmick
{
    public class DamageArea : MonoBehaviour, IGimmickID
    {
        public GimmickID GimmickIdInfo { get; set; }
        public event Action<IGimmickID> Returned;
        
        private PlayerHpHandler _playerHpHandler;
        [SerializeField] private int _damageForSeconds;
        private const float OneSecond = 1.0f;
        private float _currentTime;
        private MetaAIHandler _metaAIHandler;
        [SerializeField] private MetaAIHandler.AddScores[] _metaAiScores;

        private void Start()
        {
            _metaAIHandler = GameObject.FindWithTag("MetaAI").GetComponent<MetaAIHandler>();
        }
        
        private void OnDestroy()
        {
            Returned?.Invoke(this);
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
            if (!_playerHpHandler.IsAddDamage) return;
            _playerHpHandler.ReceiveDamage(_damageForSeconds);
            _currentTime = OneSecond; 
            _metaAIHandler.SendLogsForMetaAI(_metaAiScores);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            _currentTime = 0;
        }
    }
}
