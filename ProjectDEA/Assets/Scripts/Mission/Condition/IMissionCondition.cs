using UnityEngine;

namespace Mission.Condition
{
    public interface IMissionCondition
    {
        string MissionName { get; }
        ClassType ClassType { get; }
        MissionType MissionType { get; }
        event System.Action OnMissionCompleted;
        void StartTracking();
        void StopTracking();
        void OnDefeated(int id);
        public int CurrentCount { get; }
        public int MaxCount { get; }
        public string[] MissionLaunchLog { get; }
        public string[] MissionFinishLog { get; }
        public GameObject StandOutTarget { get; }
        public void PlayerChangeRoomEvent();
    }
    public enum ClassType
    {
        EnemyKill,
        Gimmick,
        UseItem
    }
    
    public enum MissionType
    {
        None,
        Killer,
        Achiever,
        Explorer
    }
}