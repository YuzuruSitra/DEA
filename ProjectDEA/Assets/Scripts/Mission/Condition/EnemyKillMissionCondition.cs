using Gimmick;
using Mission.CreateScriptableObject;
using UnityEngine;

namespace Mission.Condition
{
    public class EnemyKillMissionCondition : IMissionCondition
    {
        public event System.Action OnMissionCompleted;
        
        public string MissionName { get; }
        public ClassType ClassType { get; }
        public MissionType MissionType { get; }
        private readonly RoomGimmickGenerator _roomGimmickGenerator;
        private readonly int _targetEnemyID;
        private readonly EnemyKillMissionData.GenerateType _generateType;
        private readonly GameObject[] _enemyPrefab;
        public string[] MissionLaunchLog { get; }
        public string[] MissionFinishLog { get; }

        public int CurrentCount { get; private set; }
        public int MaxCount { get; private set; }
        public GameObject StandOutTarget { get; private set; }

        public EnemyKillMissionCondition(RoomGimmickGenerator roomGimmickGenerator, GameObject[] enemyPrefab, EnemyKillMissionData.KillMissionStruct enemyKillMissionData)
        {
            _roomGimmickGenerator = roomGimmickGenerator;
            _enemyPrefab = enemyPrefab;
            MissionName = enemyKillMissionData._missionName;
            ClassType = enemyKillMissionData._classType;
            MissionType = enemyKillMissionData._missionType;
            _targetEnemyID = enemyKillMissionData._targetEnemyID;
            _generateType = enemyKillMissionData._generateType;
            MaxCount = enemyKillMissionData._targetKillCount;
            MissionLaunchLog = enemyKillMissionData._missionLaunchLog;
            MissionFinishLog = enemyKillMissionData._missionFinishLog;
        }

        public void StartTracking()
        {
            CurrentCount = 0;
            StandOutTarget = null;
            GenerateEnemy();
        }

        public void StopTracking()
        {

        }

        public void PlayerChangeRoomEvent()
        {
            
        }

        public void OnDefeated(int id)
        {
            if (_targetEnemyID != EnemyKillMissionData.NonTargetID && id != _targetEnemyID) return;
            CurrentCount++;

            if (CurrentCount >= MaxCount)
            {
                OnMissionCompleted?.Invoke();
            }
        }

        private void GenerateEnemy()
        {
            var actualCount = 0;
            for (var i = 0; i < MaxCount; i++)
            {
                var targetEnemy = GetTargetEnemy();
                var targetRoom = GetTargetRoom();
                var insEnemy = _roomGimmickGenerator.InsGimmick(targetRoom, targetEnemy);
                if (insEnemy != null) actualCount++;
                if (StandOutTarget == null) StandOutTarget = insEnemy;
            }
            MaxCount = actualCount;
        }
        
        private GameObject GetTargetEnemy()
        {
            return _targetEnemyID switch
            {
                EnemyKillMissionData.NonTargetID => _enemyPrefab[Random.Range(0, _enemyPrefab.Length)],
                _ => _enemyPrefab[_targetEnemyID]
            };
        }

        private int GetTargetRoom()
        {
            return _generateType switch
            {
                EnemyKillMissionData.GenerateType.RandomRoom => _roomGimmickGenerator.GetRandomRoom,
                EnemyKillMissionData.GenerateType.ObeliskRoom => _roomGimmickGenerator.GetObeliskRoom,
                _ => 0
            };
        }
    }
}

