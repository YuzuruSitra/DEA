using System.Collections.Generic;
using Mission.Condition;
using UnityEngine.AddressableAssets;
using Mission.CreateScriptableObject;

namespace Mission
{
    public class MissionInitializer
    {
        private readonly GameEventManager _gameEventManager;
        public List<IMissionCondition> MissionConditions { get; }
        
        public MissionInitializer(GameEventManager gameEventManager)
        {
            MissionConditions = new List<IMissionCondition>();
            _gameEventManager = gameEventManager;
            InitializeMissionConditions();
        }

        private void InitializeMissionConditions()
        {
            InstanceKillEnemiesMission();
        }

        private async void InstanceKillEnemiesMission()
        {
            const string key = "Assets/Addressables_Resources/KillEnemiesData.asset";
            var handle = Addressables.LoadAssetAsync<EnemyKillMissionData>(key);
            var data = await handle.Task;
            var killEnemiesMission = new EnemyKillMissionCondition(_gameEventManager, data);
            AddMissionList(killEnemiesMission);
        }

        private void AddMissionList(IMissionCondition missionCondition)
        {
            MissionConditions.Add(missionCondition);
        }
        
        
    }
}