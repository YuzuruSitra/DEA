namespace Character.NPC
{
    public interface INpcAiState
    {
        bool IsStateFin { get; }
        void EnterState();
        void UpdateState();
        void ExitState();
    }

    public enum PartnerAIState
    {
        Stay,
        Follow,
        FreeWalk,
        Event
    }
}