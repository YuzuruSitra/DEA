using System;
using System.Collections.Generic;
using Manager.MetaAI;
using Mission.Condition;
using Random = UnityEngine.Random;

namespace Mission
{
    public class MissionSelector
    {
        private readonly MissionInitializer _missionInitializer;
        
        public MissionSelector(MissionInitializer missionInitializer)
        {
            _missionInitializer = missionInitializer;
        }
        
        public IMissionCondition SelectMission()
        {
            // メタAIの影響を一旦削除
            var typeCount = Enum.GetValues(typeof(MetaAIHandler.PlayerType)).Length - 1;
            var type = (MetaAIHandler.PlayerType)Random.Range(0, typeCount);
            // プレイヤータイプとミッションリストのマッピング
            var missionDict = new Dictionary<MetaAIHandler.PlayerType, List<IMissionCondition>>
            {
                { MetaAIHandler.PlayerType.Killer, _missionInitializer.KillerMissions },
                { MetaAIHandler.PlayerType.Achiever, _missionInitializer.AchieverMissions },
                { MetaAIHandler.PlayerType.Explorer, _missionInitializer.ExplorerMissions }
            };

            // 選択したタイプのミッションリストが存在するかチェック
            if (missionDict.TryGetValue(type, out var missions) && missions.Count > 0)
            {
                return missions[Random.Range(0, missions.Count)];
            }
            return null;
        }
    }
}