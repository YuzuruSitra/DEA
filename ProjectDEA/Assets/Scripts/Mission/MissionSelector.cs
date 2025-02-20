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
        private readonly MetaAIHandler _metaAIHandler;
        
        public MissionSelector(MissionInitializer missionInitializer, MetaAIHandler metaAIHandler)
        {
            _missionInitializer = missionInitializer;
            _metaAIHandler = metaAIHandler;
        }
        
        public IMissionCondition SelectMission()
        {
            var type = _metaAIHandler.CurrentPlayerType;
    
            if (type == MetaAIHandler.PlayerType.None)
            {
                var typeCount = Enum.GetValues(typeof(MetaAIHandler.PlayerType)).Length - 1;
                type = (MetaAIHandler.PlayerType)Random.Range(0, typeCount);
            }

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