using System;
using Gimmick;
using Manager;
using Manager.MetaAI;
using Mission.Condition;
using UnityEngine;

namespace Mission
{
    public class MissionStateHandler
    {
        private readonly MissionSelector _missionSelector;
        public IMissionCondition CurrentMission { get; private set; }

        private readonly InventoryHandler _inventoryHandler;
        public bool DoingMission => CurrentMission != null;
        public Action OnMissionStarted;
        public Action OnMissionFinished;

        public MissionStateHandler(GameEventManager gameEventManager, RoomGimmickGenerator roomGimmickGenerator, InventoryHandler inventoryHandler, MetaAIHandler metaAIHandler)
        {
            var missionInitializer = new MissionInitializer(gameEventManager, roomGimmickGenerator, inventoryHandler);
            _missionSelector = new MissionSelector(missionInitializer, metaAIHandler);
            OnMissionFinished += inventoryHandler.RemoveMissionItem;
            _inventoryHandler = inventoryHandler;
        }
        
        public void Dispose()
        {
            // イベントの購読を解除
            OnMissionFinished -= _inventoryHandler.RemoveMissionItem;
        }
        
        // ミッションを特定の条件で開始させる。
        public void StartMission()
        {
            CurrentMission = _missionSelector.SelectMission();
            Debug.Log("Mission selected: " + CurrentMission.MissionName);
            CurrentMission.OnMissionCompleted += CompleteMission;
            CurrentMission.StartTracking();
            OnMissionStarted?.Invoke();
        }

        private void CompleteMission()
        {
            if (CurrentMission == null) return;
            CurrentMission.OnMissionCompleted -= CompleteMission;
            CurrentMission.StopTracking();
            CurrentMission = null;
            OnMissionFinished?.Invoke();
            Debug.Log("ミッション達成！");
        }
    }
}

