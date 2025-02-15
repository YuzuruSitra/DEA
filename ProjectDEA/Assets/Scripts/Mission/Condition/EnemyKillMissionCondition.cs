using Gimmick;
using Mission.CreateScriptableObject;
using Test.NPC;
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
        private readonly int _targetKillCount;
        private int _currentKillCount;
        private EnemyKillMissionData.GenerateType _generateType;
        private readonly NpcController[] _enemyPrefab;

        public EnemyKillMissionCondition(GameEventManager gameEventManager, RoomGimmickGenerator roomGimmickGenerator, NpcController[] enemyPrefab, EnemyKillMissionData.KillMissionStruct enemyKillMissionData)
        {
            _gameEventManager = gameEventManager;
            _roomGimmickGenerator = roomGimmickGenerator;
            _enemyPrefab = enemyPrefab;
            MissionName = enemyKillMissionData._missionName;
            MissionType = enemyKillMissionData._missionType;
            _targetEnemyID = enemyKillMissionData._enemyID;
            _currentKillCount = 0;
            _generateType = enemyKillMissionData._generateType;
            _targetKillCount = enemyKillMissionData._targetKillCount;
        }

        public void StartTracking()
        {
            _gameEventManager.OnEnemyDefeated += OnEnemyDefeated;
            GenerateEnemy();
        }

        public void StopTracking()
        {
            _gameEventManager.OnEnemyDefeated -= OnEnemyDefeated;
        }

        private void OnEnemyDefeated(int enemyID)
        {
            if (enemyID != _targetEnemyID) return;

            _currentKillCount++;
            Debug.Log($"敵討伐ミッション進捗: {_currentKillCount}/{_targetKillCount}");

            if (_currentKillCount >= _targetKillCount)
            {
                OnMissionCompleted?.Invoke();
            }
        }

        private void GenerateEnemy()
        {
            // enemyKillMissionData.
            // _enemyPrefab[_targetEnemyID]
            Debug.Log(_roomGimmickGenerator.ObeliskRoom);
        }
        
    }
}

