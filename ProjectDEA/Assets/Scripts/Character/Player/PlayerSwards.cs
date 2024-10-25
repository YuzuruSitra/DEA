using Character.NPC.EnemyDragon;
using UnityEngine;

namespace Character.Player
{
    public class PlayerSwards : MonoBehaviour
    {
        [SerializeField] private PlayerAttackHandler _playerAttackHandler;
        private GameObject _currentDragon;
        private DragonController _currentDragonController;
        private bool _oneHit;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!_playerAttackHandler.IsAttacking)
            {
                _oneHit = false;
                return;
            }
            if (_oneHit) return;
            if (!other.CompareTag("EnemyDragon")) return;
            if (_currentDragon != other.gameObject)
            {
                _currentDragon = other.gameObject;
                _currentDragonController = other.gameObject.GetComponent<DragonController>();
            }
            _currentDragonController.OnGetDamage(_playerAttackHandler.AttackDamage, _playerAttackHandler.gameObject.transform.position);
            _oneHit = true;
        }
    }
}
