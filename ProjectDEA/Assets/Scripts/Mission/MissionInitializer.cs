using System;
using System.Collections.Generic;
using Gimmick;
using Manager;
using Mission.Condition;
using UnityEngine.AddressableAssets;
using Mission.CreateScriptableObject;

namespace Mission
{
    public class MissionInitializer
    {
        private const string MissionKeyHolder = "Assets/Addressables_Resources/MissionKeyHolder.asset";
        private readonly GameEventManager _gameEventManager;
        private readonly RoomGimmickGenerator _roomGimmickGenerator;
        private readonly InventoryHandler _inventoryHandler;
        public List<IMissionCondition> KillerMissions { get; }
        public List<IMissionCondition> AchieverMissions { get; }
        public List<IMissionCondition> ExplorerMissions { get; }
        
        public MissionInitializer(GameEventManager gameEventManager, RoomGimmickGenerator roomGimmickGenerator, InventoryHandler inventoryHandler)
        {
            KillerMissions = new List<IMissionCondition>();
            AchieverMissions = new List<IMissionCondition>();
            ExplorerMissions = new List<IMissionCondition>();
            _gameEventManager = gameEventManager;
            _roomGimmickGenerator = roomGimmickGenerator;
            _inventoryHandler = inventoryHandler;
            InitializeMissionConditions();
        }

        private void InitializeMissionConditions()
        {
            var keyHolder = LoadKeyHolder();
            InstanceKillEnemiesMission(keyHolder._enemyKillMissionData);
            InstanceGimmickMission(keyHolder._gimmickMissionData);
            InstanceUseItemMission(keyHolder._useItemMissionData);
        }

        private MissionKeyHolder LoadKeyHolder()
        {
            var handle = Addressables.LoadAssetAsync<MissionKeyHolder>(MissionKeyHolder);
            return handle.WaitForCompletion();
        }

        private void InstanceKillEnemiesMission(EnemyKillMissionData dates)
        {
            foreach (var t in dates._killMissionData)
            {
                var killEnemiesMission = new EnemyKillMissionCondition(_gameEventManager, _roomGimmickGenerator, dates._enemyPrefab, t);
                AddMissionList(killEnemiesMission);
            }
        }
        
        private void InstanceGimmickMission(GimmickMissionData dates)
        {
            foreach (var t in dates._gimmickMissionData)
            {
                var gimmickMission = new GimmickMissionCondition(_gameEventManager, _roomGimmickGenerator, dates._gimmickPrefab, t);
                AddMissionList(gimmickMission);
            }
        }
        
        private void InstanceUseItemMission(UseItemMissionData dates)
        {
            foreach (var t in dates._useItemMissionData)
            {
                var gimmickMission = new UseItemMissionCondition(_gameEventManager, _roomGimmickGenerator, _inventoryHandler, dates._itemData, t);
                AddMissionList(gimmickMission);
            }
        }

        private void AddMissionList(IMissionCondition missionCondition)
        {
            switch (missionCondition.MissionType)
            {
                case MissionType.Killer:
                    KillerMissions.Add(missionCondition);
                    break;
                case MissionType.Achiever:
                    AchieverMissions.Add(missionCondition);
                    break;
                case MissionType.Explorer:
                    ExplorerMissions.Add(missionCondition);
                    break;
                case MissionType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        
    }
}