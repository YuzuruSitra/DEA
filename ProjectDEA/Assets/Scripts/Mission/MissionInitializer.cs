using System.Collections.Generic;
using Mission.Condition;
using UnityEngine.AddressableAssets;
using Mission.CreateScriptableObject;

namespace Mission
{
    public class MissionInitializer
    {
        private const string MissionKeyHolder = "Assets/Addressables_Resources/MissionKeyHolder.asset";
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
            var keyHolder = LoadKeyHolder();
            InstanceKillEnemiesMission(keyHolder._enemyKillMissionData);
        }

        private MissionKeyHolder LoadKeyHolder()
        {
            var handle = Addressables.LoadAssetAsync<MissionKeyHolder>(MissionKeyHolder);
            return handle.WaitForCompletion();
        }

        private void InstanceKillEnemiesMission(EnemyKillMissionData dates)
        {
            var enemyPrefab = dates._enemyPrefab;
            for (var i = 0; i < enemyPrefab.Length; i++)
            {
                enemyPrefab[i].enemyID = i;
            }
            foreach (var t in dates._killMissionData)
            {
                var killEnemiesMission = new EnemyKillMissionCondition(_gameEventManager, enemyPrefab, t);
                AddMissionList(killEnemiesMission);
            }
        }

        private void AddMissionList(IMissionCondition missionCondition)
        {
            MissionConditions.Add(missionCondition);
        }
        
        
    }
}