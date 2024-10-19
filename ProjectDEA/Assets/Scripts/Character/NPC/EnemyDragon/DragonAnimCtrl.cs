using UnityEngine;

namespace Character.NPC.EnemyDragon
{
    public class DragonAnimCtrl : MonoBehaviour
    {
        public enum AnimState
        {
            Idole,
            IsWalk,
            IsRun,
            IsScream
        }
        [SerializeField] private Animator _animator;
        [SerializeField] private DragonController _dragonController;
        private static readonly int Idle = Animator.StringToHash("IsIdle");
        private static readonly int Walk = Animator.StringToHash("IsWalk");
        private static readonly int Run = Animator.StringToHash("IsRun");
        private static readonly int Scream = Animator.StringToHash("IsScream");
        private static readonly int IsGetHit = Animator.StringToHash("IsGetHit");

        private void Start()
        {
            _dragonController.GetDamage += GetHitAnim;
        }

        private void OnDestroy()
        {
            _dragonController.GetDamage -= GetHitAnim;
        }

        private void GetHitAnim()
        {
            _animator.SetTrigger(IsGetHit);   
        }

        private void Update()
        {
            switch (_dragonController.AnimState)
            {
                case AnimState.Idole:
                    AllAnimOff();
                    _animator.SetBool(Idle, true);
                    break;
                case AnimState.IsWalk:
                    AllAnimOff();
                    _animator.SetBool(Walk, true);
                    break;
                case AnimState.IsRun:
                    AllAnimOff();
                    _animator.SetBool(Run, true);
                    break;
                case AnimState.IsScream:
                    AllAnimOff();
                    _animator.SetBool(Scream, true);
                    break;
            }
        }

        private void AllAnimOff()
        {
            _animator.SetBool(Idle, false);
            _animator.SetBool(Walk, false);
            _animator.SetBool(Run, false);
            _animator.SetBool(Scream, false);
        }
        
    }
}
