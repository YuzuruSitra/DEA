using UnityEngine;

namespace Character.NPC.State
{
    public interface IUtilityAction
    {
        UtilityActionType ActionType { get; }
        float CalculateUtility();
        void EnterState();
        void Execute(GameObject agent);
        void ExitState();
    }
    
    public enum UtilityActionType
    {
        Roaming,
        Rest,
        Battle
    }
    
    public interface IBattleSubState
    {
        float CalculateUtility();
        void EnterState(Transform targetTransform);
        void Execute();
        void ExitState();
    }
    
}
