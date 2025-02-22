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
            public ClassType _classType;
            public MissionType _missionType;
            public int _missionItemID;
            public int _targetCompleteCount;
            public GameObject[] _addEnemyPrefab;
            public int _addEnemyCount;
            public string[] _missionLaunchLog;
            public string[] _missionFinishLog;
            public bool _isDynamicAddEnemy;
        }
        public UseItemMissionStruct[] _useItemMissionData;
    }
}