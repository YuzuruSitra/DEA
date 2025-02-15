using Gimmick;
using Mission.CreateScriptableObject;
using Test.NPC;
using Unity.VisualScripting;
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
        private readonly EnemyKillMissionData.GenerateType _generateType;
        private readonly GameObject[] _enemyPrefab;

        public EnemyKillMissionCondition(GameEventManager gameEventManager, RoomGimmickGenerator roomGimmickGenerator, GameObject[] enemyPrefab, EnemyKillMissionData.KillMissionStruct enemyKillMissionData)
        {
            _gameEventManager = gameEventManager;
            _roomGimmickGenerator = roomGimmickGenerator;
            _enemyPrefab = enemyPrefab;
            MissionName = enemyKillMissionData._missionName;
            MissionType = enemyKillMissionData._missionType;
            _targetEnemyID = enemyKillMissionData._targetEnemyID;
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
            for (var i = 0; i < _targetKillCount; i++)
            {
                var targetEnemy = GetTargetEnemy();
                var targetRoom = GetTargetRoom();
                _roomGimmickGenerator.InsGimmick(targetRoom, targetEnemy);
                Debug.Log("generate : " + targetEnemy.name + " Room : " + targetRoom);
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

