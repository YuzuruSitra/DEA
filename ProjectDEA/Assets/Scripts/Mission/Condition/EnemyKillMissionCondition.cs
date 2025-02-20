using Gimmick;
using Mission.CreateScriptableObject;
using UnityEngine;

namespace Mission.Condition
{
    public class EnemyKillMissionCondition : IMissionCondition
    {
        public event System.Action OnMissionCompleted;
        
        public string MissionName { get; }
        public MissionType MissionType { get; }
        private readonly GameEventManager _gameEventManager;
        private readonly RoomGimmickGenerator _roomGimmickGenerator;
        private readonly int _targetEnemyID;
        private readonly EnemyKillMissionData.GenerateType _generateType;
        private readonly GameObject[] _enemyPrefab;
        public string[] MissionLaunchLog { get; }
        public string[] MissionFinishLog { get; }

        public int CurrentCount { get; private set; }
        public int MaxCount { get; }
        public GameObject StandOutTarget { get; private set; }

        public EnemyKillMissionCondition(GameEventManager gameEventManager, RoomGimmickGenerator roomGimmickGenerator, GameObject[] enemyPrefab, EnemyKillMissionData.KillMissionStruct enemyKillMissionData)
        {
            _gameEventManager = gameEventManager;
            _roomGimmickGenerator = roomGimmickGenerator;
            _enemyPrefab = enemyPrefab;
            MissionName = enemyKillMissionData._missionName;
            MissionType = enemyKillMissionData._missionType;
            _targetEnemyID = enemyKillMissionData._targetEnemyID;
            _generateType = enemyKillMissionData._generateType;
            MaxCount = enemyKillMissionData._targetKillCount;
            MissionLaunchLog = enemyKillMissionData._missionLaunchLog;
            MissionFinishLog = enemyKillMissionData._missionFinishLog;
        }

        public void StartTracking()
        {
            _gameEventManager.OnEnemyDefeated += OnEnemyDefeated;
            CurrentCount = 0;
            StandOutTarget = null;
            GenerateEnemy();
        }

        public void StopTracking()
        {
            _gameEventManager.OnEnemyDefeated -= OnEnemyDefeated;
        }

        private void OnEnemyDefeated(int enemyID)
        {
            if (enemyID != _targetEnemyID) return;

            CurrentCount++;

            if (CurrentCount >= MaxCount)
            {
                OnMissionCompleted?.Invoke();
            }
        }

        private void GenerateEnemy()
        {
            for (var i = 0; i < MaxCount; i++)
            {
                var targetEnemy = GetTargetEnemy();
                var targetRoom = GetTargetRoom();
                var insEnemy = _roomGimmickGenerator.InsGimmick(targetRoom, targetEnemy);
                if (StandOutTarget == null) StandOutTarget = insEnemy;
            }
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

