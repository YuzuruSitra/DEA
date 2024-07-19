using System;
using System.Collections.Generic;
using System.Debug;
using Player;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using System.Map;

namespace PartnerNPC
{
    public class PartnerNpcController : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private PartnerAIState _currentState;
        private readonly Dictionary<PartnerAIState, IPartnerAIState> _states = new();
        private readonly Dictionary<PartnerAIState, int> _utilities = new()
        {
            { PartnerAIState.Stay, 50 },
            { PartnerAIState.Follow, 0 },
            { PartnerAIState.FreeWalk, 50 },
            { PartnerAIState.Event, 0 }
        };
        private DebugColor _debugColor;
        private PlayerMover _playerMover;
        private InRoomChecker _inRoomChecker;
        
        private void Start()
        {
            var player = GameObject.FindWithTag("Player");
            _playerMover = player.GetComponent<PlayerMover>();
            _debugColor = new DebugColor(GetComponent<Renderer>().material);
            _agent = GetComponent<NavMeshAgent>();
            _inRoomChecker = new InRoomChecker();
            _states.Add(PartnerAIState.Stay, new StayState(gameObject));
            _states.Add(PartnerAIState.Follow, new FollowState(player, _agent));
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
            OutRoomFollow();
        }

        private void NextState()
        {
            // ÈÅ∏ÂÆöÂ?¶Áê?„ÇíÊåü„ÇÄ
            UpdateUtilities();
            var newState = SelectState();
            // „É?„Éê„ÉÉ„Ç∞Âá¶Áê?
            DebugColor(newState);
            _states[_currentState].ExitState();
            _states[newState].EnterState();
            _currentState = newState;
        }

        private PartnerAIState SelectState()
        {
            if (_utilities.Count == 0) return PartnerAIState.Stay;

            var maxUtility = int.MinValue;
            var selectedState = PartnerAIState.Stay;
            var countMaxUtility = 0;

            // Iterate through _utilities to find the state with the highest utility
            foreach (var utility in _utilities)
            {
                if (utility.Value > maxUtility)
                {
                    maxUtility = utility.Value;
                    selectedState = utility.Key;
                    countMaxUtility = 1;
                }
                else if (utility.Value == maxUtility)
                {
                    countMaxUtility++;
                    if (Random.Range(0, countMaxUtility) == 0)
                        selectedState = utility.Key;
                }
            }
            return selectedState;
        }

        private void UpdateUtilities()
        {
            var keysToUpdate = new List<PartnerAIState>(_utilities.Keys); // Create a list of keys to iterate over
            foreach (var key in keysToUpdate)
            {
                if (key == _currentState)
                    _utilities[key] = key is PartnerAIState.Event or PartnerAIState.Follow ? 0 : _utilities[key] -= 25;
                else
                    _utilities[key] = key is PartnerAIState.Follow ? 0 : _utilities[key] += 10;
                _utilities[key] = Math.Clamp(_utilities[key], 0, 100);
            }
        }

        private void OutRoomFollow()
        {
            var stayRoom = _inRoomChecker.CheckStayRoomNum(transform.position);
            var playerStayRoom = _playerMover.StayRoomNum;
            if (stayRoom != playerStayRoom || stayRoom == InRoomChecker.RoadNum)
            {
                if (_currentState == PartnerAIState.Follow) return;
                var keysToUpdate = new List<PartnerAIState>(_utilities.Keys);
                foreach (var key in keysToUpdate)
                {
                    if (key == PartnerAIState.Follow)
                        _utilities[key] = 100;
                    else
                        _utilities[key] -= 10;
                    _utilities[key] = Math.Clamp(_utilities[key], 0, 100);
                }
                DebugColor(PartnerAIState.Follow);
                _states[_currentState].ExitState();
                _states[PartnerAIState.Follow].EnterState();
                _currentState = PartnerAIState.Follow;
            }
            else
            {
                if (_currentState != PartnerAIState.Follow) return;
                NextState();
            }
        }

        private void DebugColor(PartnerAIState newState)
        {
            switch (newState)
            {
                // „É?„Éê„ÉÉ„Ç∞Âá¶Áê?
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
