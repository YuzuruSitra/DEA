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
        private readonly int _targetEnemyID;
        private readonly int _targetKillCount;
        private int _currentKillCount;
        private readonly NpcController[] _enemyPrefab;

        public EnemyKillMissionCondition(GameEventManager gameEventManager, NpcController[] enemyPrefab, EnemyKillMissionData.KillMissionStruct enemyKillMissionData)
        {
            _enemyPrefab = enemyPrefab;
            MissionName = enemyKillMissionData._missionName;
            MissionType = enemyKillMissionData._missionType;
            _gameEventManager = gameEventManager;
            _targetEnemyID = enemyKillMissionData._enemyID;
            _currentKillCount = 0;
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
        }
        
    }
}

