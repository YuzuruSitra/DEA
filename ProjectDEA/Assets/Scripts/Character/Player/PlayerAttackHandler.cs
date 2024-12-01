using System;
using Manager.Audio;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character.Player
{
    public class PlayerAttackHandler : MonoBehaviour
    {
        private bool _isCanAttack = true;
        [SerializeField] private float _attackCt;
        [SerializeField] private float _attackMoveTime;
        [SerializeField] private float _attackSpeed;
        private float _currentTime;
        [SerializeField] private PlayerAnimationCnt _playerAnimationCnt;
        [SerializeField] private PlayerMover _playerMover;
        [SerializeField] private int _attackDamage;
        public int AttackDamage => _attackDamage;
        public bool IsAttacking => _playerAnimationCnt.IsAttacking;
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _attackSeClip;
        private InputActions _inputActions;
        
        private void Start()
        {
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
            // InputActions をインスタンス化
            _inputActions = new InputActions();
            _inputActions.Player.Attack.performed += OnAttack;
            _inputActions.Enable();
        }

        private void OnDestroy()
        {
            _inputActions.Player.Attack.performed -= OnAttack;
            _inputActions.Disable();
        }

        private void Update()
        {
            if (!_isCanAttack) return;
            _currentTime -= Time.deltaTime;
        }
        
        private void OnAttack(InputAction.CallbackContext context)
        {
            if (!_isCanAttack) return;
            if (0 < _currentTime) return;
            _playerAnimationCnt.AttackActive();
            _soundHandler.PlaySe(_attackSeClip);
            StartCoroutine(_playerMover.AttackMove(_attackMoveTime, _attackSpeed));
            _currentTime = _attackCt;
        }
        
        public void SetCanAttackState(bool active)
        {
            _isCanAttack = active;
        }

        public void ChangeAttackPower(int newDamage)
        {
            _attackDamage = newDamage;
        }
    }
}
