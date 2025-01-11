using Test.NPC.Dragon;
using UnityEngine;
using AnimationState = Test.NPC.AnimatorControl.AnimationState;

namespace Test.NPC
{
    public class RestAction : IUtilityAction
    {
        public UtilityActionType ActionType => UtilityActionType.Rest;

        // Configuration
        private readonly AnimatorControl _animatorControl;
        private readonly MovementControl _movementControl;
        private readonly NpcStatusComponent _npcStatusComponent;
        private readonly Transform _agent;

        // Rest logic
        private readonly float _restSearchRange;
        private readonly float _bias;
        
        public RestAction(Transform agent, AnimatorControl animatorControl, MovementControl movementControl, NpcStatusComponent npcStatusComponent, DragonController.RestParameters restParameters)
        {
            _agent = agent;
            _animatorControl = animatorControl;
            _movementControl = movementControl;
            _npcStatusComponent = npcStatusComponent;
            _restSearchRange = restParameters._restSearchRange;
            _bias = restParameters._bias;
        }

        public float CalculateUtility()
        {
            return Mathf.Max(0, 1 - _npcStatusComponent.CurrentStamina / NpcStatusComponent.MaxStamina - _bias);
        }

        public void EnterState()
        {
            SetNewRoamingDestination();
            _animatorControl.SetAnimParameter(AnimationState.Moving);
        }

        public void Execute(GameObject agent)
        {
            if (!_movementControl.HasReachedDestination()) return;
            _npcStatusComponent.RecoverStamina();
            _animatorControl.SetAnimParameter(AnimationState.Rest);
        }

        private void SetNewRoamingDestination()
        {
            var randomOffset = new Vector3(
                Random.Range(-_restSearchRange, _restSearchRange),
                0f,
                Random.Range(-_restSearchRange, _restSearchRange)
            );
            var targetPos = _agent.position + randomOffset;
            
            if (UnityEngine.AI.NavMesh.SamplePosition(targetPos, out var hit, _restSearchRange, UnityEngine.AI.NavMesh.AllAreas))
            {
                targetPos = hit.position;
            }
            
            _movementControl.MoveTo(targetPos);
        }
    }
}
