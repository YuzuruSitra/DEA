using System;
using Mission.Condition;
using UnityEngine;

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