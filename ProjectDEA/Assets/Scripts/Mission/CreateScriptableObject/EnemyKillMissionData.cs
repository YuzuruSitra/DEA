using Mission.Condition;
using UnityEngine;

namespace Mission.CreateScriptableObject
{
    [CreateAssetMenu(fileName = "NewEnemyKillMission", menuName = "Mission/EnemyKillMission")]
    public class EnemyKillMissionData : ScriptableObject
    {
        public string _missionName;
        public MissionType _missionType;
        public int _enemyID;
        public int _targetKillCount;
    }
}