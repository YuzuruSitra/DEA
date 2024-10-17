using UnityEngine;

namespace Character.Player
{
    public class PlayerAnimationCnt : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IsDamaged = Animator.StringToHash("IsDamaged");
        
        public void SetSpeed(float speedRatio)
        {
            _animator.SetFloat(Speed, speedRatio);
        }

        public void SetIsDamaged(bool active)
        {
            _animator.SetBool(IsDamaged, active);
        }
        
        
    }
}