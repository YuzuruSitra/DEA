using Gimmick;
using Mission.CreateScriptableObject;
using UnityEngine;

namespace Mission.Condition
{
    public class UseItemMissionCondition : IMissionCondition
    {
        public event System.Action OnMissionCompleted;
        
        public string MissionName { get; }
        public MissionType MissionType { get; }
        private readonly GameEventManager _gameEventManager;
        private readonly RoomGimmickGenerator _roomGimmickGenerator;
        private readonly int _targetGimmickID;
        private readonly int _targetCount;
        private int _currentCompleteCount;
        private readonly GimmickMissionData.GenerateType _generateType;
        private readonly GameObject[] _enemyPrefab;
        
        public UseItemMissionCondition(GameEventManager gameEventManager, RoomGimmickGenerator roomGimmickGenerator, GameObject[] enemyPrefab, GimmickMissionData.GimmickMissionStruct gimmickMissionStruct)
        {
            _gameEventManager = gameEventManager;
            _roomGimmickGenerator = roomGimmickGenerator;
            _enemyPrefab = enemyPrefab;
            MissionName = gimmickMissionStruct._missionName;
            MissionType = gimmickMissionStruct._missionType;
            _targetGimmickID = gimmickMissionStruct._targetGimmickID;
            _currentCompleteCount = 0;
            _generateType = gimmickMissionStruct._generateType;
            _targetCount = gimmickMissionStruct._targetCompleteCount;
        }

        public void StartTracking()
        {
            _gameEventManager.OnGimmickCompleted += OnGimmickCompleted;
            GenerateEnemy();
        }

        public void StopTracking()
        {
            _gameEventManager.OnGimmickCompleted -= OnGimmickCompleted;
        }

        private void OnGimmickCompleted(int gimmickID)
        {
            if (gimmickID != _targetGimmickID) return;

            _currentCompleteCount++;
            Debug.Log($"ギミックミッション進捗: {_currentCompleteCount}/{_targetCount}");

            if (_currentCompleteCount >= _targetCount)
            {
                OnMissionCompleted?.Invoke();
            }
        }
        
        private void GenerateEnemy()
        {
            for (var i = 0; i < _targetCount; i++)
            {
                var targetEnemy = GetTargetEnemy();
                var targetRoom = GetTargetRoom();
                _roomGimmickGenerator.InsGimmick(targetRoom, targetEnemy);
                Debug.Log("generate : " + targetEnemy.name + " Room : " + targetRoom);
            }
        }
        
        private GameObject GetTargetEnemy()
        {
            return _targetGimmickID switch
            {
                EnemyKillMissionData.NonTargetID => _enemyPrefab[Random.Range(0, _enemyPrefab.Length)],
                _ => _enemyPrefab[_targetGimmickID]
            };
        }

        private int GetTargetRoom()
        {
            return _generateType switch
            {
                GimmickMissionData.GenerateType.RandomRoom => _roomGimmickGenerator.GetRandomRoom,
                GimmickMissionData.GenerateType.ObeliskRoom => _roomGimmickGenerator.GetObeliskRoom,
                _ => 0
            };
        }
    }
}

