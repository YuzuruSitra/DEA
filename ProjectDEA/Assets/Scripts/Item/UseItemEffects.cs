using Character.Player;
using Manager.Audio;
using UnityEngine;

namespace Item
{
    public class UseItemEffects : MonoBehaviour
    {
        [SerializeField] private PlayerClasHub _playerClasHub;
        [SerializeField] private int _powerPotionUpValue;
        [SerializeField] private float _addSpeedValue;
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _buffItemAudio;

        private void Start()
        {
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
        }

        public void PlayerPowerUpper()
        {
            var attackHandler = _playerClasHub.PlayerAttackHandler;
            attackHandler.ChangeAttackPower(attackHandler.AttackDamage + _powerPotionUpValue);
            _soundHandler.PlaySe(_buffItemAudio);
        }

        public void PlayerSpeedUpper()
        {
            var playerMover = _playerClasHub.PlayerMover;
            playerMover.ChangeSpeed(_addSpeedValue);
            _soundHandler.PlaySe(_buffItemAudio);
        }
    }
}
