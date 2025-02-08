using Mission.Condition;
using UnityEngine;

namespace Mission
{
    public class MissionStateHandler
    {
        private readonly MissionSelector _missionSelector;
        private IMissionCondition _currentMission;

        public MissionStateHandler(GameEventManager gameEventManager)
        {
            var missionInitializer = new MissionInitializer(gameEventManager);
            _missionSelector = new MissionSelector(missionInitializer);
        }
        
        // ミッションを特定の条件で開始させる。
        public void StartMission()
        {
            if (_currentMission != null)
            {
                _currentMission.StopTracking();
            }
            
            _currentMission = _missionSelector.SelectMission();
            Debug.Log("Mission selected: " + _currentMission.MissionName);
            _currentMission.OnMissionCompleted += CompleteMission;
            _currentMission.StartTracking();
        }

        private void CompleteMission()
        {
            Debug.Log("ミッション達成！");
            if (_currentMission == null) return;
            _currentMission.OnMissionCompleted -= CompleteMission;
            _currentMission.StopTracking();
            _currentMission = null;
        }
    }
}

