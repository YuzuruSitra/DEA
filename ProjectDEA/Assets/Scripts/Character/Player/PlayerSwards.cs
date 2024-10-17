using System;
using Character.NPC.EnemyDragon;
using UnityEngine;

namespace Character.Player
{
    public class PlayerSwards : MonoBehaviour
    {
        [SerializeField] private PlayerAttackHandler _playerAttackHandler;
        private int _power;

        private void Start()
        {
            _power = _playerAttackHandler.AttackDamage;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!_playerAttackHandler.IsAttacking) return;
            if (!other.CompareTag("EnemyDragon")) return;
            var dragon = other.gameObject.GetComponent<DragonController>();
            dragon.OnGetDamage(_power);
        }
    }
}
