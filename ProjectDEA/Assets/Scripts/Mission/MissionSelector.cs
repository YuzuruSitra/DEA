using System.Collections.Generic;
using Mission.Condition;
using UnityEngine;

namespace Mission
{
    public class MissionSelector
    {
        private readonly MissionInitializer _missionSelector = new();
        private List<IMissionCondition> MissionList => _missionSelector.MissionConditions;

        public IMissionCondition SelectMission()
        {
            if (MissionList.Count == 0)
            {
                Debug.LogWarning("Mission list is empty.");
                return null;
            }
            var rnd = Random.Range(0, MissionList.Count);
            return MissionList[rnd];
        }
    }
}