using System.Collections.Generic;
using Mission.Condition;
using UnityEngine;

namespace Mission
{
    public class MissionSelector
    {
        private readonly MissionInitializer _missionInitializer;
        private List<IMissionCondition> MissionList => _missionInitializer.MissionConditions;

        public MissionSelector(MissionInitializer missionInitializer)
        {
            _missionInitializer = missionInitializer;
        }
        
        public IMissionCondition SelectMission()
        {
            if (MissionList.Count == 0)
            {
                Debug.LogWarning("Mission list is empty.");
                return null;
            }

            var rnd = 0;
            //var rnd = Random.Range(0, MissionList.Count);
            return MissionList[rnd];
        }
    }
}