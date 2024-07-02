using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PartnerNPCController : MonoBehaviour
{
    NavMeshAgent _agent;
    private PartnerAIState _currentState;
    private Dictionary<PartnerAIState, IPartnerAIState> _states = new Dictionary<PartnerAIState, IPartnerAIState>();
    Dictionary<PartnerAIState, int> _utilities = new Dictionary<PartnerAIState, int>
    {
        { PartnerAIState.STAY, 0 },
        { PartnerAIState.FOLLOW, 0 },
        { PartnerAIState.FREE_WALK, 0 },
        { PartnerAIState.EVENT, 0 }
    };

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _states.Add(PartnerAIState.STAY, new StayState(gameObject));
        _states.Add(PartnerAIState.FOLLOW, new FollowState(GameObject.FindWithTag("Player"), _agent));
        _states.Add(PartnerAIState.FREE_WALK, new FreeWalkState(gameObject, _agent));
        _states.Add(PartnerAIState.EVENT, new EventState());

        _currentState = PartnerAIState.FREE_WALK;
        _states[_currentState].EnterState();
        DebugColor(_currentState);
    }

    void Update()
    {
        _states[_currentState].UpdateState();
        if (_states[_currentState].IsStateFin) NextState();
    }

    void NextState()
    {
        // 選定処理を挟む
        UpdateUtilities();
        PartnerAIState newState = SelectState();
        // デバッグ処理
        DebugColor(newState);
        _states[_currentState].ExitState();
        _states[newState].EnterState();
        _currentState = newState;
    }

    PartnerAIState SelectState()
    {
        PartnerAIState selectedState = PartnerAIState.STAY;
        int maxUtility = int.MinValue;
        foreach (var utility in _utilities)
        {
            if (utility.Value > maxUtility)
            {
                maxUtility = utility.Value;
                selectedState = utility.Key;
            }
        }
        return selectedState;
    }

    public void UpdateUtilities()
    {
        _utilities[PartnerAIState.STAY] = Random.Range(0, 100);
        _utilities[PartnerAIState.FOLLOW] = Random.Range(0, 100);
        _utilities[PartnerAIState.FREE_WALK] = Random.Range(0, 100);
        //_utilities[PartnerAIState.EVENT] = Random.Range(0, 100);
    }

    private void DebugColor(PartnerAIState newState)
    {
        // デバッグ処理
        DebugColor _debugColor = new DebugColor(GetComponent<Renderer>().material);
        switch (newState)
        {
            case PartnerAIState.STAY:
                _debugColor.ChangeColor(Color.blue);
                break;
            case PartnerAIState.FOLLOW:
                _debugColor.ChangeColor(Color.cyan);
                break;
            case PartnerAIState.FREE_WALK:
                _debugColor.ChangeColor(Color.green);
                break;
            case PartnerAIState.EVENT:
                _debugColor.ChangeColor(Color.red);
                break;
            default:
                break;
        }
    }

}
