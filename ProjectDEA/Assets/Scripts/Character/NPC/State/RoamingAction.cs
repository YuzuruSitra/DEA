using UnityEngine;
using AnimationBool = Character.NPC.EnemyAnimHandler.AnimationBool;

namespace Character.NPC.State
{
    public class RoamingAction : IUtilityAction
    {
        public UtilityActionType ActionType => UtilityActionType.Roaming;

        // Configuration
        private readonly EnemyAnimHandler _enemyAnimHandler;
        private readonly MovementControl _movementControl;
        private readonly NpcStatusComponent _npcStatusComponent;
        private readonly Transform _agent;

        // Roaming logic
        private float _roamingTimer;
        private readonly float _intervalTimeMax;
        private readonly float _intervalTimeMin;
        private readonly float _roamingSearchRange;
        private readonly float _fullnessW;
        private readonly float _staminaW;
        private readonly float _bias;
        
        public RoamingAction(Transform agent, EnemyAnimHandler enemyAnimHandler, MovementControl movementControl, NpcStatusComponent npcStatusComponent, NpcController.RoamingParameters roamingParameters)
        {
            _agent = agent;
            _enemyAnimHandler = enemyAnimHandler;
            _movementControl = movementControl;
            _npcStatusComponent = npcStatusComponent;
            _intervalTimeMax = roamingParameters._intervalTimeMax;
            _intervalTimeMin = roamingParameters._intervalTimeMin;
            _roamingSearchRange = roamingParameters._roamingSearchRange;
            _fullnessW = roamingParameters._fullnessW;
            _staminaW = roamingParameters._staminaW;
            _bias = roamingParameters._bias;
        }

        public float CalculateUtility()
        {
            var utility = _fullnessW * _npcStatusComponent.CurrentFullness / NpcStatusComponent.MaxFullness + _staminaW * _npcStatusComponent.CurrentStamina / NpcStatusComponent.MaxStamina - _bias;
            utility = Mathf.Max(0, utility);
            return utility;
        }

        public void EnterState()
        {
            _enemyAnimHandler.ChangeAnimBool(AnimationBool.Moving);
            SetNewRoamingDestination();
            SetRoamingTimer();
        }

        public void Execute(GameObject agent)
        {
            if (!_movementControl.HasReachedDestination())
            {
                _npcStatusComponent.ConsumeStamina();
                _npcStatusComponent.ConsumeFullness();
                return;
            }
            _roamingTimer -= Time.deltaTime;
            if (_roamingTimer > 0) return;
            SetNewRoamingDestination();
            SetRoamingTimer();
        }
        
        public void ExitState()
        {
            
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
