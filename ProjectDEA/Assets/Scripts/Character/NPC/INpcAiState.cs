namespace Character.NPC
{
    public interface INpcAiState
    {
        bool IsStateFin { get; }
        void EnterState();
        void UpdateState();
        void ExitState();
    }

    public enum AIState
    {
        Stay,
        Attack,
        FreeWalk
    }
}