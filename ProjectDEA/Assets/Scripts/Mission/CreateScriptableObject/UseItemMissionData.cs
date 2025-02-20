using System;
using Item;
using Mission.Condition;
using UnityEngine;

namespace Mission.CreateScriptableObject
{
    [CreateAssetMenu(fileName = "UseItemMissionData", menuName = "Mission/UseItemMission")]
    public class UseItemMissionData : ScriptableObject
    {
        public ItemKind[] _itemData;
        [Serializable]
        public struct UseItemMissionStruct
        {
            public string _missionName;
            public MissionType _missionType;
            public int _targetItemID;
            public int _targetCompleteCount;
            public GameObject[] _addEnemyPrefab;
            public int _addEnemyCount;
        } 
        public UseItemMissionStruct[] _gimmickMissionData;
        public const int NonTargetID = -1;
        
    }
}