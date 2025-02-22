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
        private readonly GameEventManager _gameEventManager;
        private readonly MissionSelector _missionSelector;
        public IMissionCondition CurrentMission { get; private set; }

        private readonly InventoryHandler _inventoryHandler;
        public bool DoingMission { get; private set; }
        public Action OnMissionStarted;
        public Action OnMissionFinished;

        public MissionStateHandler(GameEventManager gameEventManager, RoomGimmickGenerator roomGimmickGenerator, InventoryHandler inventoryHandler)
        {
            _gameEventManager = gameEventManager;
            var missionInitializer = new MissionInitializer(roomGimmickGenerator, inventoryHandler);
            _missionSelector = new MissionSelector(missionInitializer);
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
            CurrentMission.OnMissionCompleted += CompleteMission;
            _gameEventManager.OnEnemyDefeated += CurrentMission.OnDefeated;
            CurrentMission.StartTracking();
            OnMissionStarted?.Invoke();
            DoingMission = true;
        }

        private void CompleteMission()
        {
            if (CurrentMission == null) return;
            CurrentMission.OnMissionCompleted -= CompleteMission;
            _gameEventManager.OnEnemyDefeated -= CurrentMission.OnDefeated;
            CurrentMission.StopTracking();
            OnMissionFinished?.Invoke();
            DoingMission = false;
        }
    }
}

