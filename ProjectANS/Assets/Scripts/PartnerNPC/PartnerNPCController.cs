using System.Collections.Generic;
using UnityEngine;

public class PartnerNPCController : MonoBehaviour
{
    [Header("移動速度")]
    [SerializeField] 
    private float _moveSpeed;
    public float MoveSpeed => _moveSpeed;
    // [Header("目標座標に対する許容誤差")]
    // [SerializeField] 
    // private float _stoppingDistance = 0.1f;

    private PartnerAIState _currentState;
    private Dictionary<PartnerAIState, IPartnerAIState> _states = new Dictionary<PartnerAIState, IPartnerAIState>();

    //private Animator _animator;
    // ターゲット座標を保持

    // 移動用クラス
    // private InnNPCMover _innNPCMover;
    // public InnNPCMover InnNPCMover => _innNPCMover;

    void Start()
    {
        _currentState = PartnerAIState.STAY;
    }

    void Update()
    {
        _states[_currentState].UpdateState();
        // ChangeAnimWalk(_states[_currentState].IsWalk);
        if (_states[_currentState].IsStateFin) NextState(_currentState);
    }

    void NextState(PartnerAIState state)
    {
        // 選定処理を挟む
        PartnerAIState newState = state;
        switch (state)
        {
            case PartnerAIState.STAY:
                break;
            case PartnerAIState.FOLLOW:
                break;
            case PartnerAIState.FREE_WALK:
                break;
            default:
                newState = state;
                break;
        }
        _states[_currentState].ExitState();
        _states[newState].EnterState();
        _currentState = newState;
    }
}
