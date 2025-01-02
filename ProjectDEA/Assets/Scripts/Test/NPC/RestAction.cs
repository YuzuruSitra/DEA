using UnityEngine;
using AnimationState = Test.NPC.AnimatorControl.AnimationState;

namespace Test.NPC
{
    public class RestAction : IUtilityAction
    {
        public UtilityActionType ActionType => UtilityActionType.Attack;
        public AnimationState CurrentAnimState => AnimationState.Moving;
        private readonly AnimatorControl _animatorControl;
        private readonly MovementControl _movementControl;
        private readonly Vector3 _searchOffSet;
        private readonly float _searchRadius;
        private readonly LayerMask _searchLayer;
        private readonly float _attackRadius;
        private readonly float _attackDelay;
        private readonly float _attackRange;
        private readonly float _damage;
        private readonly Vector3 _attackOrigin;
        
        private readonly Transform _agent;
        private Transform _target;
        
        public RestAction(Transform agent, AnimatorControl animatorControl, MovementControl movementControl)
        {
            _agent = agent;
            _animatorControl = animatorControl;
            _movementControl = movementControl;
        }

        public float CalculateUtility()
        {
            return 0f;
        }

        public void Execute(GameObject agent)
        {
        }
    }
}
