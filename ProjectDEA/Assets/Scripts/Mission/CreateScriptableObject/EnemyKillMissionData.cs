using System;
using Mission.Condition;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mission.CreateScriptableObject
{
    [CreateAssetMenu(fileName = "NewEnemyKillMission", menuName = "Mission/EnemyKillMission")]
    public class EnemyKillMissionData : ScriptableObject
    {
        public GameObject[] _enemyPrefab;
        [Serializable]
        public struct KillMissionStruct
        {
            public string _missionName;
            public ClassType _classType;
            public MissionType _missionType;
            public int _targetEnemyID;
            public int _targetKillCount;
            public GenerateType _generateType;
            public string[] _missionLaunchLog;
            public string[] _missionFinishLog;
        } 
        public KillMissionStruct[] _killMissionData;
        public const int NonTargetID = -1;
        
        public enum GenerateType
        {
            None,
            RandomRoom,
            ObeliskRoom
        }
        
    }
}