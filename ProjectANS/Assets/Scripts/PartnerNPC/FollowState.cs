using UnityEngine;
using UnityEngine.AI;

public class FollowState : IPartnerAIState
{
    private Transform _player;
    NavMeshAgent _agent;

    public FollowState(GameObject player, NavMeshAgent agent)
    {
        _player = player.transform;
        _agent = agent;
    }

    // 仮置き
    public bool IsStateFin => false;

    // ステートに入った時の処理
    public void EnterState()
    {
        
    }

    // ステートの更新
    public void UpdateState()
    {
        _agent.destination = _player.position;
    }

    public void ExitState()
    {

    }
}
