using System;
using Mission.Condition;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mission.CreateScriptableObject
{
    [CreateAssetMenu(fileName = "NewEnemyKillMission", menuName = "Mission/EnemyKillMission")]
    public class EnemyKillMissionData : ScriptableObject
    {
        [Serializable]
        public struct KillMissionStruct
        {
            public string _missionName;
            public MissionType _missionType;
            public int _enemyID;
            public int _targetKillCount;
        } 
        public KillMissionStruct[] _killMissionData;
        
    }
}