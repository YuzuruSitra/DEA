using UnityEngine;
using System.Map;

namespace Player
{
    public class PlayerMover : MonoBehaviour
    {
        // private Transform _parent;
        [Header("歩行速度")]
        [SerializeField]
        private float _walkSpeed;
        [Header("走行速度")]
        [SerializeField]
        private float _runSpeed;
        private CharacterController _controller;
        private Vector3 _moveDirection = Vector3.zero;
        private Vector3 _direction = Vector3.zero;
        private InRoomChecker _inRoomChecker;
        public int StayRoomNum { get; private set; }
        
        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _inRoomChecker = new InRoomChecker();
        }

        private void Update()
        {
            StayRoomNum = _inRoomChecker.CheckStayRoomNum(transform.position);
            
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            
            _moveDirection.x = horizontal;
            _moveDirection.z = vertical;
            
            var speed = Input.GetKey(KeyCode.LeftShift) ? _runSpeed : _walkSpeed;
            
            if (horizontal != 0 || vertical != 0)
            {
                _direction.x = horizontal;
                _direction.z = vertical;
                _direction.y = 0;
                transform.rotation = Quaternion.LookRotation(_direction);
            }
            
            _controller.Move(_moveDirection * (speed * Time.deltaTime));
        }
    }
}