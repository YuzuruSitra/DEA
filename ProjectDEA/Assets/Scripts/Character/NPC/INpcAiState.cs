using Character.NPC.EnemyDragon;

namespace Character.NPC
{
    public interface INpcAiState
    {
        DragonAnimCtrl.AnimState CurrentAnim { get; }
        bool IsStateFin { get; }
        void EnterState();
        void UpdateState();
        void ExitState();
    }

    public enum AIState
    {
        Null,
        Stay,
        Attack,
        FreeWalk
    }
}