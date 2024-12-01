using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Character.Player
{
    public class PlayerAnimationCnt : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IsDamaged = Animator.StringToHash("IsDamaged");
        private static readonly int IsAttack = Animator.StringToHash("IsAttack");
        private static readonly int AttackType = Animator.StringToHash("AttackType");
        private static readonly int IsDie = Animator.StringToHash("IsDie");
        [SerializeField] private AnimationClip[] _attackClips;
        private WaitForSeconds[] _attackWait;
        [SerializeField] private float _attackLaunchTime;
        private WaitForSeconds _attackLaunchWait;
        private Coroutine _attackRoutine;
        public bool IsAttacking { get; private set; }
        
        private void Start()
        {
            var clipCount = _attackClips.Length;
            _attackWait = new WaitForSeconds[clipCount];
            for (var i = 0; i < clipCount; i++)
            {
                _attackWait[i] = new WaitForSeconds(_attackClips[i].length / 2.0f - _attackLaunchTime);
            }
            _attackLaunchWait = new WaitForSeconds(_attackLaunchTime);
        }

        public void SetSpeed(float speedRatio)
        {
            speedRatio = Mathf.Max(speedRatio, 0.1f);
            _animator.SetFloat(Speed, speedRatio);
        }

        public void SetIsDamaged(bool active)
        {
            _animator.SetBool(IsDamaged, active);
        }
        
        public void SetIsDie()
        {
            _animator.SetBool(IsDie, true);
        }
        
        public void AttackActive()
        {
            var attackType = Random.Range(0, 2);
            _animator.SetFloat(AttackType, attackType);
            _animator.SetBool(IsAttack, true);
            if (_attackRoutine != null) StopCoroutine(_attackRoutine);
            _attackRoutine = StartCoroutine(AttackOff(_attackWait[attackType]));
        }

        private IEnumerator AttackOff(WaitForSeconds wait)
        {
            yield return _attackLaunchWait;
            IsAttacking = true;
            yield return wait;
            _animator.SetBool(IsAttack, false);
            _attackRoutine = null;
            IsAttacking = false;
        }
    }
}