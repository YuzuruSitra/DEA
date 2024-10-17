using UnityEngine;

namespace Character.Player
{
    public class PlayerAttackHandler : MonoBehaviour
    {
        [SerializeField] private float _attackCt;
        [SerializeField] private float _attackMoveTime;
        [SerializeField] private float _attackSpeed;
        private float _currentTime;
        [SerializeField] private PlayerAnimationCnt _playerAnimationCnt;
        [SerializeField] private PlayerMover _playerMover;
        private void Update()
        {
            _currentTime -= Time.deltaTime;
            if (0 < _currentTime) return;
            if (Input.GetMouseButtonDown(0)) OnAttack();
        }
        
        private void OnAttack()
        {
            _playerAnimationCnt.AttackActive();
            StartCoroutine(_playerMover.AttackMove(_attackMoveTime, _attackSpeed));
            _currentTime = _attackCt;
        }
    }
}
