using System;
using Mission.Condition;
using Test.NPC;
using UnityEngine;

namespace Mission.CreateScriptableObject
{
    [CreateAssetMenu(fileName = "NewEnemyKillMission", menuName = "Mission/EnemyKillMission")]
    public class EnemyKillMissionData : ScriptableObject
    {
        public NpcController[] _enemyPrefab;
            
        [Serializable]
        public struct KillMissionStruct
        {
            public string _missionName;
            public MissionType _missionType;
            public int _enemyID;
            public int _targetKillCount;
            public GenerateType _generateType;
        } 
        public KillMissionStruct[] _killMissionData;

        public enum GenerateType
        {
            None,
            RandomRoom,
            PlayerRoom
        }
        
    }
}