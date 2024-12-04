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
        private Vector3 _verticalDirection = Vector3.zero;
        private Vector3 _inputDirection = Vector3.zero;
        private Vector3 _rotationDirection = Vector3.zero;
        private bool _isPushed;

        // 重力の値
        [SerializeField] private float _gravity = -9.81f;

        // 現在の移動速度
        private float _currentSpeed;

        // 速度の減速スピード
        [SerializeField] private float _decelerationRate = 5f;

        // 速度の変化にかかる時間
        [SerializeField] private float _speedTransitionTime;

        private InputActions _inputActions;
        private Vector2 _inputMove = Vector2.zero; // 再利用可能なベクトル
        private bool _isRunning;

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _controller.enabled = true;
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
            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled -= OnMoveCanceled;
            _inputActions.Player.Run.performed -= OnRunPerformed;
            _inputActions.Player.Run.canceled -= OnRunCanceled;
            _inputActions.Disable();
        }

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
            VerticalMoving();
            if (!_isWalkable || _isPushed || _playerAnimationCnt.IsAttacking) return;

            UpdateInputDirection();
            UpdateMoveDirection();
            DoMoving();
        }

        private void UpdateInputDirection()
        {
            // 入力方向を計算して再利用可能なベクトルに代入
            _inputDirection.x = _inputMove.x;
            _inputDirection.z = _inputMove.y;

            if (_inputDirection.magnitude > 1f)
                _inputDirection.Normalize();
        }

        private void UpdateMoveDirection()
        {
            // 入力がない場合は速度を徐々に減少
            if (_inputDirection == Vector3.zero)
            {
                _currentSpeed = Mathf.Lerp(_currentSpeed, 0f, Time.deltaTime * _decelerationRate);
                _moveDirection.x = Mathf.Lerp(_moveDirection.x, 0f, Time.deltaTime * _decelerationRate);
                _moveDirection.z = Mathf.Lerp(_moveDirection.z, 0f, Time.deltaTime * _decelerationRate);
            }
            else
            {
                var targetSpeed = _isRunning ? _runSpeed : _walkSpeed;
                _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.deltaTime / _speedTransitionTime);

                _moveDirection.x = _inputDirection.x * _currentSpeed;
                _moveDirection.z = _inputDirection.z * _currentSpeed;

                // 回転処理用に方向ベクトルを計算して再利用
                _rotationDirection.x = _moveDirection.x;
                _rotationDirection.z = _moveDirection.z;
                _rotationDirection.y = 0;

                if (_rotationDirection != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(_rotationDirection);
            }
        }

        private void VerticalMoving()
        {
            // 重力適用
            if (_controller.isGrounded)
                _verticalDirection.y = 0f;
            else
                _verticalDirection.y += _gravity * Time.deltaTime;
            _controller.Move(_verticalDirection * Time.deltaTime);
        }

        private void DoMoving()
        {
            // CharacterControllerで移動
            _controller.Move(_moveDirection * Time.deltaTime);

            // アニメーション用の速度比率
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
                if (halfT < elapsedTime)
                    _controller.Move(transform.forward * (speed * Time.deltaTime));
                yield return null;
            }
        }

        public IEnumerator PushMoveUp(float duration, float initialUpwardForce)
        {
            if (_playerHpHandler.IsDie) yield break;

            _isPushed = true;
            _playerAnimationCnt.SetIsDamaged(true);
            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var upwardForce = initialUpwardForce * (1 - (elapsedTime / duration));
                var upwardVelocity = Vector3.up * upwardForce;
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
