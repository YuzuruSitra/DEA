using System.Collections.Generic;
using Gimmick;
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
        public List<IMissionCondition> MissionConditions { get; }
        
        public MissionInitializer(GameEventManager gameEventManager, RoomGimmickGenerator roomGimmickGenerator)
        {
            MissionConditions = new List<IMissionCondition>();
            _gameEventManager = gameEventManager;
            _roomGimmickGenerator = roomGimmickGenerator;
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
            foreach (var t in dates._killMissionData)
            {
                var killEnemiesMission = new EnemyKillMissionCondition(_gameEventManager, _roomGimmickGenerator, dates._enemyPrefab, t);
                AddMissionList(killEnemiesMission);
            }
        }

        private void AddMissionList(IMissionCondition missionCondition)
        {
            MissionConditions.Add(missionCondition);
        }
        
        
    }
}