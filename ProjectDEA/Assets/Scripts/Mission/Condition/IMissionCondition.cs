namespace Mission.Condition
{
    public interface IMissionCondition
    {
        string MissionName { get; }
        MissionType MissionType { get; }
        event System.Action OnMissionCompleted;
        void StartTracking();
        void StopTracking();
        public int CurrentCount { get; }
        public int MaxCount { get; }
    }

    public enum MissionType
    {
        None,
        Killer,
        Achiever,
        Explorer
    }
}