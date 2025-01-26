using UnityEngine;

namespace Test.NPC.State
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
        Attack,
        Escape
    }
    
}
