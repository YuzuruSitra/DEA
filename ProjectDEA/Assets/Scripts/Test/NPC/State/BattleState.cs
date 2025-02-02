using System.Collections.Generic;
using Test.NPC.Dragon;
using UnityEngine;

namespace Test.NPC.State
{
    public class BattleState : IUtilityAction
    {
        public UtilityActionType ActionType => UtilityActionType.Battle;
        private readonly BattleStateSelector _battleStateSelector;
        private readonly Transform _agent;
        private Transform _target;
        private readonly float _searchOffSetFactor;
        private readonly float _searchRadius;
        private readonly LayerMask _searchLayer;
        private readonly Collider[] _searchResults = new Collider[1];
        private IBattleSubState _currentState;
        
        public BattleState(Transform agent, List<IBattleSubState> subStates, DragonController.BattleStateParameters battleStateParameters)
        {
            _agent = agent;
            _battleStateSelector = new BattleStateSelector(subStates);
            _searchOffSetFactor = battleStateParameters._searchOffSetFactor;
            _searchRadius = battleStateParameters._searchRadius;
            _searchLayer = battleStateParameters._searchLayer;
        }
        
        public float CalculateUtility()
        {
            _target = FindTarget(_agent.position + _agent.forward * _searchOffSetFactor, _searchRadius);
            return _target != null ? 1f : 0f;
        }

        public void EnterState()
        {
            var newState = _battleStateSelector.SelectBestAction();
            if (_currentState == newState) return;
            _currentState = newState;
            _currentState.EnterState(_agent);
        }

        public void Execute(GameObject agent)
        {
            _currentState.Execute();
        }

        public void ExitState()
        {
            _currentState?.ExitState();
        }
        
        private Transform FindTarget(Vector3 origin, float radius)
        {
            var count = Physics.OverlapSphereNonAlloc(origin, radius, _searchResults, _searchLayer, QueryTriggerInteraction.Ignore);
            return (count > 0 && _searchResults[0] != null) ? _searchResults[0].transform : null;
        }
    
    }
}
