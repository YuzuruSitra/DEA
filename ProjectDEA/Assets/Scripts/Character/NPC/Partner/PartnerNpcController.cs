using System;
using System.Collections.Generic;
using Manager.Debug;
using Manager.Map;
using Player;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Character.NPC.Partner
{
    public class PartnerNpcController : MonoBehaviour
    {
        // private NavMeshAgent _agent;
        // private AIState _currentState;
        // private readonly Dictionary<AIState, INpcAiState> _states = new();
        // private readonly Dictionary<AIState, int> _utilities = new()
        // {
        //     { AIState.Stay, 50 },
        //     { AIState.Attack, 0 },
        //     { AIState.FreeWalk, 50 },
        // };
        // private DebugColor _debugColor;
        // private PlayerMover _playerMover;
        // private InRoomChecker _inRoomChecker;
        //
        // private void Start()
        // {
        //     var player = GameObject.FindWithTag("Player");
        //     _playerMover = player.GetComponent<PlayerMover>();
        //     _debugColor = new DebugColor(GetComponent<Renderer>().material);
        //     _agent = GetComponent<NavMeshAgent>();
        //     _inRoomChecker = new InRoomChecker();
        //     _states.Add(AIState.Stay, new StayState(gameObject));
        //     _states.Add(AIState.Attack, new FollowState(player, _agent));
        //     _states.Add(AIState.FreeWalk, new FreeWalkState(gameObject, _agent));
        //
        //     _currentState = AIState.FreeWalk;
        //     _states[_currentState].EnterState();
        //     DebugColor(_currentState);
        // }
        //
        // private void Update()
        // {
        //     _states[_currentState].UpdateState();
        //     if (_states[_currentState].IsStateFin) NextState();
        //     OutRoomFollow();
        // }
        //
        // private void NextState()
        // {
        //     // 選定�?��?を挟む
        //     UpdateUtilities();
        //     var newState = SelectState();
        //     // �?バッグ処�?
        //     DebugColor(newState);
        //     _states[_currentState].ExitState();
        //     _states[newState].EnterState();
        //     _currentState = newState;
        // }
        //
        // private AIState SelectState()
        // {
        //     if (_utilities.Count == 0) return AIState.Stay;
        //
        //     var maxUtility = int.MinValue;
        //     var selectedState = AIState.Stay;
        //     var countMaxUtility = 0;
        //
        //     // Iterate through _utilities to find the state with the highest utility
        //     foreach (var utility in _utilities)
        //     {
        //         if (utility.Value > maxUtility)
        //         {
        //             maxUtility = utility.Value;
        //             selectedState = utility.Key;
        //             countMaxUtility = 1;
        //         }
        //         else if (utility.Value == maxUtility)
        //         {
        //             countMaxUtility++;
        //             if (Random.Range(0, countMaxUtility) == 0)
        //                 selectedState = utility.Key;
        //         }
        //     }
        //     return selectedState;
        // }
        //
        // private void UpdateUtilities()
        // {
        //     var keysToUpdate = new List<AIState>(_utilities.Keys); // Create a list of keys to iterate over
        //     foreach (var key in keysToUpdate)
        //     {
        //         if (key == _currentState)
        //             _utilities[key] = key is AIState.Attack ? 0 : _utilities[key] -= 25;
        //         else
        //             _utilities[key] = key is AIState.Attack ? 0 : _utilities[key] += 10;
        //         _utilities[key] = Math.Clamp(_utilities[key], 0, 100);
        //     }
        // }
        //
        // private void OutRoomFollow()
        // {
        //     var stayRoom = _inRoomChecker.CheckStayRoomNum(transform.position);
        //     var playerStayRoom = _playerMover.StayRoomNum;
        //     if (stayRoom != playerStayRoom || stayRoom == InRoomChecker.RoadNum)
        //     {
        //         if (_currentState == AIState.Attack) return;
        //         var keysToUpdate = new List<AIState>(_utilities.Keys);
        //         foreach (var key in keysToUpdate)
        //         {
        //             if (key == AIState.Attack)
        //                 _utilities[key] = 100;
        //             else
        //                 _utilities[key] -= 10;
        //             _utilities[key] = Math.Clamp(_utilities[key], 0, 100);
        //         }
        //         DebugColor(AIState.Attack);
        //         _states[_currentState].ExitState();
        //         _states[AIState.Attack].EnterState();
        //         _currentState = AIState.Attack;
        //     }
        //     else
        //     {
        //         if (_currentState != AIState.Attack) return;
        //         NextState();
        //     }
        // }
        //
        // private void DebugColor(AIState newState)
        // {
        //     switch (newState)
        //     {
        //         // �?バッグ処�?
        //         case AIState.Stay:
        //             _debugColor.ChangeColor(Color.blue);
        //             break;
        //         case AIState.Attack:
        //             _debugColor.ChangeColor(Color.cyan);
        //             break;
        //         case AIState.FreeWalk:
        //             _debugColor.ChangeColor(Color.green);
        //             break;
        //     }
        // }
    }
}
