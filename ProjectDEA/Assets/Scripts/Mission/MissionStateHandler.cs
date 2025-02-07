using Mission.Condition;
using UnityEngine;

namespace Mission
{
    public class MissionStateHandler
    {
        private readonly MissionSelector _missionSelector = new();
        private IMissionCondition _currentMission;

        // ミッションを特定の条件で開始させる。
        public void StartMission(IMissionCondition mission)
        {
            if (_currentMission != null)
            {
                _currentMission.StopTracking();
            }

            _currentMission = _missionSelector.SelectMission();
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

