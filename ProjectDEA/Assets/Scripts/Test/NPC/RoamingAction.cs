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
        private Vector3 _currentDestination;
        private float _roamingTimer;
        private const float RoamingInterval = 5f; // Time to wait before picking a new destination
        private const float RoamingRange = 5f; // Maximum roaming range

        public RoamingAction(Transform agent, AnimatorControl animatorControl, MovementControl movementControl)
        {
            _agent = agent;
            _animatorControl = animatorControl;
            _movementControl = movementControl;
            _roamingTimer = RoamingInterval;
            _currentDestination = _agent.position;
        }

        public float CalculateUtility()
        {
            return 1f;
        }

        public void Execute(GameObject agent)
        {
            if (!_movementControl.HasReachedDestination()) return;
            _roamingTimer -= Time.deltaTime;
            if (_roamingTimer > 0) return;
            SetNewRoamingDestination();
            _movementControl.MoveTo(_currentDestination);
            _roamingTimer = RoamingInterval;
            _animatorControl.SetAnimParameter(AnimationState.Moving);
        }

        private void SetNewRoamingDestination()
        {
            // Generate a random destination within the roaming range
            var randomOffset = new Vector3(
                Random.Range(-RoamingRange, RoamingRange),
                0f,
                Random.Range(-RoamingRange, RoamingRange)
            );
            _currentDestination = _agent.position + randomOffset;

            // Ensure the destination is on the NavMesh
            if (UnityEngine.AI.NavMesh.SamplePosition(_currentDestination, out UnityEngine.AI.NavMeshHit hit, RoamingRange, UnityEngine.AI.NavMesh.AllAreas))
            {
                _currentDestination = hit.position;
            }
        }
    }
}
