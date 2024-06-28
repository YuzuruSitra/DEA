using System.Collections.Generic;
using UnityEngine;

public class PartnerNPCController : MonoBehaviour
{
    [Header("ˆÚ“®‘¬“x")]
    [SerializeField] 
    private float _moveSpeed;
    public float MoveSpeed => _moveSpeed;

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
        _currentState = PartnerAIState.STAY;
    }

    void Update()
    {
        _states[_currentState].UpdateState();
        if (_states[_currentState].IsStateFin) NextState();
    }

    void NextState()
    {
        // ‘I’èˆ—‚ð‹²‚Þ
        UpdateUtilities();
        PartnerAIState newState = SelectState();
        switch (newState)
        {
            case PartnerAIState.STAY:
                break;
            case PartnerAIState.FOLLOW:
                break;
            case PartnerAIState.FREE_WALK:
                break;
            default:
                break;
        }
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
        _utilities[PartnerAIState.EVENT] = Random.Range(0, 100);
    }

}
