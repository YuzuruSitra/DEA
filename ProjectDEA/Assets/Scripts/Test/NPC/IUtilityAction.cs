using UnityEngine;

namespace Test.NPC
{
    public interface IUtilityAction
    {
        UtilityActionType ActionType { get; }
        float CalculateUtility();
        void EnterState();
        void Execute(GameObject agent);
    }
    
    public enum UtilityActionType
    {
        Roaming,
        Rest,
        Attack,
        Escape
    }
    
}
