using UnityEngine;

namespace Test.NPC
{
    public interface IUtilityAction
    {
        UtilityActionType ActionType { get; }
        float CalculateUtility();
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
