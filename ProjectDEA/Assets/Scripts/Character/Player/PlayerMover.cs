using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character.Player
{
    public class PlayerMover : MonoBehaviour
    {
        private bool _isWalkable = true;

        [Header("歩行速度")]
        [SerializeField] private float _walkSpeed;
        [Header("走行速度")]
        [SerializeField] private float _runSpeed;
        private CharacterController _controller;

        [SerializeField] private PlayerAnimationCnt _playerAnimationCnt;
        [SerializeField] private PlayerHpHandler _playerHpHandler;

        private Vector3 _moveDirection = Vector3.zero;
        private Vector3 _direction = Vector3.zero;
        private bool _inWater;
        private bool _isPushed;

        // 重力の値
        [SerializeField] private float _gravity = -9.81f;

        // 現在の移動速度
        private float _currentSpeed;

        // 速度の変化にかかる時間
        [SerializeField] private float _speedTransitionTime;

        private InputActions _inputActions;
        private Vector2 _inputMove;
        private bool _isRunning;
        
        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _controller.enabled = true;
            // InputActions をインスタンス化
            _inputActions = new InputActions();

            // MoveとRunのアクションを購読
            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled += OnMoveCanceled;
            _inputActions.Player.Run.performed += OnRunPerformed;
            _inputActions.Player.Run.canceled += OnRunCanceled;
            _inputActions.Enable();
        }
        
        private void OnDestroy()
        {
            // リスナーを解除
            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled -= OnMoveCanceled;
            _inputActions.Player.Run.performed -= OnRunPerformed;
            _inputActions.Player.Run.canceled -= OnRunCanceled;
            _inputActions.Disable();
        }

        // リスナー用のコールバック
        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            _inputMove = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _inputMove = Vector2.zero;
        }

        private void OnRunPerformed(InputAction.CallbackContext context)
        {
            _isRunning = true;
        }

        private void OnRunCanceled(InputAction.CallbackContext context)
        {
            _isRunning = false;
        }

        private void Update()
        {
            if (!_isWalkable) return;
            if (_isPushed) return;
            if (_playerAnimationCnt.IsAttacking) return;

            DoMoving();
        }

        private void DoMoving()
        {
            var isDie = _playerHpHandler.IsDie;
            var horizontal = isDie ? 0 : _inputMove.x;
            var vertical = isDie ? 0 : _inputMove.y;

            _moveDirection.x = horizontal;
            _moveDirection.z = vertical;

            // 走るか歩くかの目標速度を設定
            var targetSpeed = _isRunning ? _runSpeed : _walkSpeed;

            // Falling
            if (!_controller.isGrounded && !_inWater)
            {
                _moveDirection.y += _gravity * Time.deltaTime;
            }
            else
            {
                if (horizontal == 0 && vertical == 0) targetSpeed = 0;
            }

            // 現在の速度を補間
            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.deltaTime / _speedTransitionTime);

            // 回転処理
            if (horizontal != 0 || vertical != 0)
            {
                _direction.x = horizontal;
                _direction.z = vertical;
                _direction.y = 0;
                transform.rotation = Quaternion.LookRotation(_direction);
            }

            // 移動処理
            _controller.Move(_moveDirection * (_currentSpeed * Time.deltaTime));
            var speedRatio = _currentSpeed / _runSpeed;
            _playerAnimationCnt.SetSpeed(speedRatio);
        }

        public IEnumerator AttackMove(float time, float speed)
        {
            var elapsedTime = 0f;
            var halfT = time / 2.0f;
            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                if (halfT < elapsedTime) _controller.Move(transform.forward * (speed * Time.deltaTime));
                yield return null;
            }
        }

        public IEnumerator PushMoveUp(float duration, float initialUpwardForce)
        {
            var isDie = _playerHpHandler.IsDie;
            if (isDie) yield break;
            _isPushed = true;
            _playerAnimationCnt.SetIsDamaged(true);
            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                var upwardForce = initialUpwardForce * (1 - (elapsedTime / duration));

                var upwardVelocity = Vector3.zero;
                upwardVelocity.y = upwardForce;

                _controller.Move(upwardVelocity * Time.deltaTime);

                yield return null;
            }
            _playerAnimationCnt.SetIsDamaged(false);
            _isPushed = false;
        }

        public void SetWalkableState(bool active)
        {
            _isWalkable = active;
        }
    }
}
