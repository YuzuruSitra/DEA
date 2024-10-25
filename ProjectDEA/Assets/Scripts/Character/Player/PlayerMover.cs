using System.Collections;
using UnityEngine;

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

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _controller.enabled = true;
        }

        private void Update()
        {
            if (!_isWalkable) return;
            if (_isPushed) return;
            if (_playerAnimationCnt.IsAttacking) return;
            InputMove();
        }

        private void InputMove()
        {
            var isDie = _playerHpHandler.IsDie;
            var horizontal = isDie? 0 : Input.GetAxis("Horizontal");
            var vertical = isDie? 0 : Input.GetAxis("Vertical");
            
            _moveDirection.x = horizontal;
            _moveDirection.z = vertical;
            
            // 走るか歩くかの目標速度を設定
            var targetSpeed = Input.GetKey(KeyCode.LeftShift) ? _runSpeed : _walkSpeed;
            // Falling.
            if (!_controller.isGrounded && !_inWater)
            {
                _moveDirection.y += _gravity * Time.deltaTime;
            }
            else
            {
                if (horizontal == 0 && vertical == 0) targetSpeed = 0;
            }
            // 現在の速度を 0.1 秒かけて目標速度に向かって補間
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
        
        // プレイヤーを滑らかに上昇させるコルーチン
        public IEnumerator PushMoveUp(float duration, float initialUpwardForce)
        {
            var isDie = _playerHpHandler.IsDie;
            if (isDie) yield break;
            _isPushed = true;
            _playerAnimationCnt.SetIsDamaged(true);
            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // 経過時間を加算
                elapsedTime += Time.deltaTime;

                // 残り時間に応じて力を減少させる (最初が最大で、時間が経つにつれて減少)
                var upwardForce = initialUpwardForce * (1 - (elapsedTime / duration));

                // 上向きの速度を計算
                var upwardVelocity = Vector3.zero;
                upwardVelocity.y = upwardForce;

                // Moveメソッドを使って移動させる
                _controller.Move(upwardVelocity * Time.deltaTime);

                // 次のフレームまで待つ
                yield return null;
            }
            _playerAnimationCnt.SetIsDamaged(false);
            _isPushed = false;
        }

        public void ChangeSpeed(float changeValue)
        {
            _walkSpeed += changeValue;
            _runSpeed += changeValue;
        }

        public void SetWalkableState(bool active)
        {
            _isWalkable = active;
        }
    }
}
