using Test.NPC.Dragon;
using UnityEngine;
using AnimationBool = Test.NPC.AnimatorControl.AnimationBool;

namespace Test.NPC.State
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
        private readonly float _targetAddStamina;
        private readonly float _waitSleepTime;
        // in state logic
        private int _targetStamina;
        private const int ResetTargetStamina = -1;
        private float _currentWaitTime;
        
        public RestAction(Transform agent, AnimatorControl animatorControl, MovementControl movementControl, NpcStatusComponent npcStatusComponent, DragonController.RestParameters restParameters)
        {
            _agent = agent;
            _animatorControl = animatorControl;
            _movementControl = movementControl;
            _npcStatusComponent = npcStatusComponent;
            _restSearchRange = restParameters._restSearchRange;
            _bias = restParameters._bias;
            _targetAddStamina = restParameters._targetAddStamina;
            _waitSleepTime = restParameters._waitSleepTime;
        }

        public float CalculateUtility()
        {
            if (_npcStatusComponent.CurrentStamina < _targetStamina)
            {
                return 1 - _bias;
            }
            return Mathf.Max(0, 1 - _npcStatusComponent.CurrentStamina / NpcStatusComponent.MaxStamina - _bias);
        }

        public void EnterState()
        {
            _targetStamina = (int)Mathf.Min(_npcStatusComponent.CurrentStamina + _targetAddStamina, NpcStatusComponent.MaxStamina);
            SetNewRoamingDestination();
            _animatorControl.ChangeAnimBool(AnimationBool.Moving);
            _currentWaitTime = _waitSleepTime;
        }

        public void Execute(GameObject agent)
        {
            if (!_movementControl.HasReachedDestination()) return;

            if (_currentWaitTime > 0) 
            { 
                _currentWaitTime -= Time.deltaTime;
                return;
            }
            _npcStatusComponent.RecoverStamina();
            _animatorControl.ChangeAnimBool(AnimationBool.Rest);
        }
        
        public void ExitState()
        {
            _targetStamina = ResetTargetStamina;
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
