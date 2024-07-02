using System;
using System.Collections.Generic;
using System.Debug;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace PartnerNPC
{
    public class PartnerNpcController : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private PartnerAIState _currentState;
        private readonly Dictionary<PartnerAIState, IPartnerAIState> _states = new Dictionary<PartnerAIState, IPartnerAIState>();
        private readonly Dictionary<PartnerAIState, int> _utilities = new Dictionary<PartnerAIState, int>
        {
            { PartnerAIState.Stay, 0 },
            { PartnerAIState.Follow, 0 },
            { PartnerAIState.FreeWalk, 0 },
            { PartnerAIState.Event, 0 }
        };
        private DebugColor _debugColor;
        
        private void Start()
        {
            _debugColor = new DebugColor(GetComponent<Renderer>().material);
            _agent = GetComponent<NavMeshAgent>();
            _states.Add(PartnerAIState.Stay, new StayState(gameObject));
            _states.Add(PartnerAIState.Follow, new FollowState(GameObject.FindWithTag("Player"), _agent));
            _states.Add(PartnerAIState.FreeWalk, new FreeWalkState(gameObject, _agent));
            _states.Add(PartnerAIState.Event, new EventState());

            _currentState = PartnerAIState.FreeWalk;
            _states[_currentState].EnterState();
            DebugColor(_currentState);
        }

        private void Update()
        {
            _states[_currentState].UpdateState();
            if (_states[_currentState].IsStateFin) NextState();
        }

        private void NextState()
        {
            // 選定処理を挟む
            UpdateUtilities();
            var newState = SelectState();
            // デバッグ処理
            DebugColor(newState);
            _states[_currentState].ExitState();
            _states[newState].EnterState();
            _currentState = newState;
        }

        private PartnerAIState SelectState()
        {
            var selectedState = PartnerAIState.Stay;
            var maxUtility = int.MinValue;
            foreach (var utility in _utilities)
            {
                if (utility.Value <= maxUtility) continue;
                maxUtility = utility.Value;
                selectedState = utility.Key;
            }
            return selectedState;
        }

        private void UpdateUtilities()
        {
            _utilities[PartnerAIState.Stay] = Random.Range(0, 100);
            _utilities[PartnerAIState.Follow] = Random.Range(0, 100);
            _utilities[PartnerAIState.FreeWalk] = Random.Range(0, 100);
            //_utilities[PartnerAIState.EVENT] = Random.Range(0, 100);
        }

        private void DebugColor(PartnerAIState newState)
        {
            switch (newState)
            {
                // デバッグ処理
                case PartnerAIState.Stay:
                    _debugColor.ChangeColor(Color.blue);
                    break;
                case PartnerAIState.Follow:
                    _debugColor.ChangeColor(Color.cyan);
                    break;
                case PartnerAIState.FreeWalk:
                    _debugColor.ChangeColor(Color.green);
                    break;
                case PartnerAIState.Event:
                    _debugColor.ChangeColor(Color.red);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

    }
}
