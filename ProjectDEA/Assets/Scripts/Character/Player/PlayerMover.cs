using UnityEngine;
using Manager.Map;

namespace Player
{
    public class PlayerMover : MonoBehaviour
    {
        [Header("歩行速度")]
        [SerializeField] private float _walkSpeed;
        [Header("走行速度")]
        [SerializeField] private float _runSpeed;
        private CharacterController _controller;
        private Vector3 _moveDirection = Vector3.zero;
        private Vector3 _direction = Vector3.zero;
        private InRoomChecker _inRoomChecker;
        public int StayRoomNum { get; private set; }
        private bool _inWater;

        // 重力の値
        [SerializeField] private float _gravity = -9.81f;

        // 現在の移動速度
        private float _currentSpeed;
        public float CurrentSpeedRatio => _currentSpeed / _runSpeed;

        // 速度の変化にかかる時間
        [SerializeField] private float _speedTransitionTime;

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _controller.enabled = true;
            _inRoomChecker = new InRoomChecker();
        }

        private void Update()
        {
            StayRoomNum = _inRoomChecker.CheckStayRoomNum(transform.position);
            
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            
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
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Water"))
            {
                _inWater = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Water"))
            {
                _inWater = false;
            }
        }
    }
}
