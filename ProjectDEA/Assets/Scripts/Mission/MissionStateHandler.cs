using System;
using Gimmick;
using Mission.Condition;
using UnityEngine;

namespace Mission
{
    public class MissionStateHandler
    {
        private readonly MissionSelector _missionSelector;
        private IMissionCondition _currentMission;
        public bool DoingMission => _currentMission != null;
        public Action OnMissionFinished;

        public MissionStateHandler(GameEventManager gameEventManager, RoomGimmickGenerator roomGimmickGenerator)
        {
            var missionInitializer = new MissionInitializer(gameEventManager, roomGimmickGenerator);
            _missionSelector = new MissionSelector(missionInitializer);
        }
        
        // ミッションを特定の条件で開始させる。
        public void StartMission()
        {
            _currentMission = _missionSelector.SelectMission();
            Debug.Log("Mission selected: " + _currentMission.MissionName);
            _currentMission.OnMissionCompleted += CompleteMission;
            _currentMission.StartTracking();
        }

        private void CompleteMission()
        {
            if (_currentMission == null) return;
            _currentMission.OnMissionCompleted -= CompleteMission;
            _currentMission.StopTracking();
            _currentMission = null;
            OnMissionFinished?.Invoke();
            Debug.Log("ミッション達成！");
        }
    }
}

