using Character.Player;
using Manager.Audio;
using Manager.MetaAI;
using UnityEngine;

namespace Item
{
    public class UseItemEffects : MonoBehaviour
    {
        [SerializeField] private PlayerClasHub _playerClasHub;
        [SerializeField] private int _powerPotionUpValue;
        [SerializeField] private int _addHpValue;
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _buffItemAudio;
        private MetaAIHandler _metaAIHandler;
        [SerializeField] private MetaAIHandler.AddScores[] _buffScores;
        
        private void Start()
        {
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
            _metaAIHandler = GameObject.FindWithTag("MetaAI").GetComponent<MetaAIHandler>();
        }

        public void PlayerPowerUpper()
        {
            var attackHandler = _playerClasHub.PlayerAttackHandler;
            attackHandler.ChangeAttackPower(attackHandler.AttackDamage + _powerPotionUpValue);
            _metaAIHandler.SendLogsForMetaAI(_buffScores);
            _soundHandler.PlaySe(_buffItemAudio);
        }

        public void PlayerSpeedUpper()
        {
            var playerHpHandler = _playerClasHub.PlayerHpHandler;
            playerHpHandler.ReceiveDamage(-_addHpValue);
            _metaAIHandler.SendLogsForMetaAI(_buffScores);
            _soundHandler.PlaySe(_buffItemAudio);
        }
    }
}
