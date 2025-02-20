using System;
using Mission.Condition;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mission.CreateScriptableObject
{
    [CreateAssetMenu(fileName = "GimmickMission", menuName = "Mission/GimmickMission")]
    public class GimmickMissionData : ScriptableObject
    {
        public GameObject[] _gimmickPrefab;
        [Serializable]
        public struct GimmickMissionStruct
        {
            public string _missionName;
            public MissionType _missionType;
            public int _targetGimmickID;
            public int _targetCompleteCount;
            public GenerateType _generateType;
            public GameObject[] _addGimmickPrefab;
            public int _addGimmickCount;
            public string[] _missionLaunchLog;
            public string[] _missionFinishLog;
        } public GimmickMissionStruct[] _gimmickMissionData;
        public const int NonTargetID = -1;
        
        public enum GenerateType
        {
            None,
            RandomRoom,
            ObeliskRoom
        }
        
    }
}