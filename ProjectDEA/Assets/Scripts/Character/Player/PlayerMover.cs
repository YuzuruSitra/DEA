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
        [SerializeField] private Rigidbody _rb;

        [SerializeField] private PlayerAnimationCnt _playerAnimationCnt;
        [SerializeField] private PlayerHpHandler _playerHpHandler;

        private Vector3 _moveDirection = Vector3.zero;
        private Vector3 _inputDirection = Vector3.zero;
        private bool _isPushed;
        private const float PushedDuration = 0.5f;
        
        [SerializeField] private float _gravity = -9.81f;
        private float _currentSpeed;
        [SerializeField] private float _decelerationRate = 5f;
        [SerializeField] private float _speedTransitionTime;

        private InputActions _inputActions;
        private Vector2 _inputMove = Vector2.zero;
        private bool _isRunning;

        private void Start()
        {
            _inputActions = new InputActions();
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

        private void FixedUpdate()
        {
            if (!_isWalkable || _isPushed || _playerAnimationCnt.IsAttacking)
            {
                _playerAnimationCnt.SetSpeed(0);
                _rb.velocity = Vector3.zero; 
                return;
            }
            
            UpdateInputDirection();
            UpdateMoveDirection();
        }

        private void UpdateInputDirection()
        {
            _inputDirection.x = _inputMove.x;
            _inputDirection.z = _inputMove.y;

            if (_inputDirection.magnitude > 1f)
            {
                _inputDirection.Normalize();
            }
        }

        private void UpdateMoveDirection()
        {
            if (_inputDirection == Vector3.zero)
            {
                _currentSpeed = Mathf.Lerp(_currentSpeed, 0f, Time.fixedDeltaTime * _decelerationRate);
            }
            else
            {
                var targetSpeed = _isRunning ? _runSpeed : _walkSpeed;
                _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.fixedDeltaTime / _speedTransitionTime);
            }
            _moveDirection = new Vector3(_inputDirection.x, 0, _inputDirection.z) * _currentSpeed;
            _moveDirection.y = _rb.velocity.y + _gravity * Time.fixedDeltaTime; 
            _rb.velocity = _moveDirection;

            if (_inputDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(new Vector3(_moveDirection.x, 0, _moveDirection.z));
            }

            _playerAnimationCnt.SetSpeed(_currentSpeed / _runSpeed);
        }

        public IEnumerator AttackMove(float time, float speed)
        {
            var elapsedTime = 0f;
            var halfT = time / 2.0f;
            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                if (halfT < elapsedTime)
                    _rb.velocity = transform.forward * speed;
                yield return null;
            }
        }

        public void LaunchPushForceMove(Vector3 direction, float initialUForce)
        {
            StartCoroutine(PushForceMove(direction, initialUForce));
        }

        private IEnumerator PushForceMove(Vector3 direction, float initialUForce)
        {
            if (_playerHpHandler.IsDie) yield break;

            _isPushed = true;
            _playerAnimationCnt.SetIsDamaged(true);
            var elapsedTime = 0f;

            while (elapsedTime < PushedDuration)
            {
                elapsedTime += Time.deltaTime;
                var force = initialUForce * (1 - (elapsedTime / PushedDuration));
                _rb.velocity = direction * force;
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