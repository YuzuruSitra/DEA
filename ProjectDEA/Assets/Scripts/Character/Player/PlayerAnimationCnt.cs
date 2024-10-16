using UnityEngine;

namespace Player
{
    public class PlayerAnimationCnt : MonoBehaviour
    {
        [SerializeField] private PlayerMover _playerMover;
        [SerializeField] private Animator _animator;
        private static readonly int Speed = Animator.StringToHash("Speed");

        private void Update()
        {
            _animator.SetFloat(Speed, _playerMover.CurrentSpeedRatio);
        }
        
    }
}