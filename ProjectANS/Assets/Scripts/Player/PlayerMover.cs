using UnityEngine;

namespace Player
{
    public class PlayerMover : MonoBehaviour
    {
        [Header("歩行速度")]
        [SerializeField]
        private float _walkSpeed;
        [Header("走行速度")]
        [SerializeField]
        private float _runSpeed;
        [Header("重力係数")]
        [SerializeField]
        private float _gravity;
        private CharacterController _controller;
        private Vector3 _moveDirection = Vector3.zero;
        private Vector3 _direction = Vector3.zero;

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            // 入力
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            // 移動方向の設定
            _moveDirection.x = horizontal;
            _moveDirection.z = vertical;

            // スピードの設定
            var speed = Input.GetKey(KeyCode.LeftShift) ? _runSpeed : _walkSpeed;

            // 重力の適用
            if (_controller.isGrounded)
                _moveDirection.y = 0;
            _moveDirection.y -= _gravity * Time.deltaTime;

            // 回転の設定
            if (horizontal != 0 || vertical != 0)
            {
                _direction.x = horizontal;
                _direction.z = vertical;
                _direction.y = 0; // 念のため、Y軸の回転をリセット
                transform.rotation = Quaternion.LookRotation(_direction);
            }

            // 移動の適用
            _controller.Move(_moveDirection * (speed * Time.deltaTime));
        }
    }
}