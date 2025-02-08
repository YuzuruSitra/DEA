using UnityEngine;

namespace Mission.CreateScriptableObject
{
    [CreateAssetMenu(fileName = "MissionKeyHolder", menuName = "Mission/MissionKeyHolder")]
    public class MissionKeyHolder : ScriptableObject
    {
        public EnemyKillMissionData[] _enemyKillMissionData;
    }
}
