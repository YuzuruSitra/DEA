using Character.Player;
using UnityEngine;

namespace Item
{
    public class UseItemEffects : MonoBehaviour
    {
        [SerializeField] private PlayerClasHub _playerClasHub;
        [SerializeField] private int _powerPotionUpValue;
        [SerializeField] private float _addSpeedValue;
        public void PlayerPowerUpper()
        {
            var attackHandler = _playerClasHub.PlayerAttackHandler;
            attackHandler.ChangeAttackPower(attackHandler.AttackDamage + _powerPotionUpValue);
        }

        public void PlayerSpeedUpper()
        {
            var playerMover = _playerClasHub.PlayerMover;
            playerMover.ChangeSpeed(_addSpeedValue);
        }
    }
}
