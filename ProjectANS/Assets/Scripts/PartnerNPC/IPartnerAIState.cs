public interface IPartnerAIState
{
    bool IsStateFin { get; }
    void EnterState();
    void UpdateState();
    void ExitState();
}

public enum PartnerAIState
{
    STAY,
    FOLLOW,
    FREE_WALK,
    EVENT
}
