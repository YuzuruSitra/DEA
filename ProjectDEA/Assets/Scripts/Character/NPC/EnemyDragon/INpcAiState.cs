namespace Character.NPC.EnemyDragon
{
    public interface INpcAiState
    {
        DragonAnimCtrl.AnimState CurrentAnim { get; }
        bool IsStateFin { get; }
        bool IsAttacking { get; }
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