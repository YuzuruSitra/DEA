using System;
using UI;
using UnityEngine;

namespace Character.Player
{
    public class UseItemEffects : MonoBehaviour
    {
        [SerializeField] private PlayerClasHub _playerClasHub;
        [SerializeField] private int _powerPotionUpValue;

        public void PlayerPowerUpper()
        {
            var attackHandler = _playerClasHub.PlayerAttackHandler;
            attackHandler.ChangeAttackPower(attackHandler.AttackDamage + _powerPotionUpValue);
        }
    }
}
