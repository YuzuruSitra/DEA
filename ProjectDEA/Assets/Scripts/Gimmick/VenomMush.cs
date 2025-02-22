using System;
using Character.Player;
using Manager.Audio;
using Manager.MetaAI;
using Mission;
using UnityEngine;

namespace Gimmick
{
    public class VenomMush : MonoBehaviour, IGimmickID
    {
        [SerializeField] private int _gimmickID;
        private PlayerHpHandler _playerHpHandler;
        private SoundHandler _soundHandler;
        public GimmickID GimmickIdInfo { get; set; }
        public event Action<IGimmickID> Returned;
        [SerializeField] private int _damageForSeconds;
        private const float OneSecond = 1.0f;
        private float _currentTime;
        [SerializeField] private AudioClip _breakedSe;
        private GameEventManager _gameEventManager;
        private MetaAIHandler _metaAIHandler;
        [SerializeField] private MetaAIHandler.AddScores[] _metaAiScores;
        
        private void Start()
        {
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
            _gameEventManager = GameObject.FindWithTag("GameEventManager").GetComponent<GameEventManager>();
            _metaAIHandler = GameObject.FindWithTag("MetaAI").GetComponent<MetaAIHandler>();
        }
        
        private void OnDestroy()
        {
            _metaAIHandler.SendLogsForMetaAI(_metaAiScores);
            _gameEventManager.GimmickCompleted(_gimmickID);
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
            if (!_playerHpHandler.IsAddDamage) return;
            _playerHpHandler.ReceiveDamage(_damageForSeconds);
            _currentTime = OneSecond;   
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            _currentTime = 0;
        }
        
    }
}
