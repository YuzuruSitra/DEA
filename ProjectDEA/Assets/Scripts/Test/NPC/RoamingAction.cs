using Test.NPC.Dragon;
using UnityEngine;
using AnimationState = Test.NPC.AnimatorControl.AnimationState;

namespace Test.NPC
{
    public class RoamingAction : IUtilityAction
    {
        public UtilityActionType ActionType => UtilityActionType.Roaming;

        // Configuration
        private readonly AnimatorControl _animatorControl;
        private readonly MovementControl _movementControl;
        private readonly Transform _agent;

        // Roaming logic
        private float _roamingTimer;
        private readonly float _intervalTimeMax;
        private readonly float _intervalTimeMin;
        private readonly float _roamingSearchRange;

        public RoamingAction(Transform agent, AnimatorControl animatorControl, MovementControl movementControl, DragonController.RoamingParameters roamingParameters)
        {
            _agent = agent;
            _animatorControl = animatorControl;
            _movementControl = movementControl;
            _intervalTimeMax = roamingParameters._intervalTimeMax;
            _intervalTimeMin = roamingParameters._intervalTimeMin;
            _roamingSearchRange = roamingParameters._roamingSearchRange;
        }

        public float CalculateUtility()
        {
            return 1f;
        }
        
        public void EnterState()
        {
            SetNewRoamingDestination();
            SetRoamingTimer();
        }

        public void Execute(GameObject agent)
        {
            if (!_movementControl.HasReachedDestination()) return;
            _roamingTimer -= Time.deltaTime;
            if (_roamingTimer > 0) return;
            SetNewRoamingDestination();
            SetRoamingTimer();
            _animatorControl.SetAnimParameter(AnimationState.Moving);
        }

        private void SetNewRoamingDestination()
        {
            var randomOffset = new Vector3(
                Random.Range(-_roamingSearchRange, _roamingSearchRange),
                0f,
                Random.Range(-_roamingSearchRange, _roamingSearchRange)
            );
            var targetPos = _agent.position + randomOffset;
            
            if (UnityEngine.AI.NavMesh.SamplePosition(targetPos, out var hit, _roamingSearchRange, UnityEngine.AI.NavMesh.AllAreas))
            {
                targetPos = hit.position;
            }
            
            _movementControl.MoveTo(targetPos);
        }

        private void SetRoamingTimer()
        {
            _roamingTimer = Random.Range(_intervalTimeMin, _intervalTimeMax);
        }
        
    }
}
