namespace PartnerNPC
{
    public interface IPartnerAIState
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