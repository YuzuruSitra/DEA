using Gimmick;
using Item;
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
        private readonly int _targetItemID;
        private readonly int _targetCount;
        private int _currentCompleteCount;
        private readonly GimmickMissionData.GenerateType _generateType;
        private readonly ItemKind[] _itemKinds;
        private readonly GameObject[] _addEnemyPrefab;
        private readonly int _addEnemyCount;
        
        public UseItemMissionCondition(GameEventManager gameEventManager, RoomGimmickGenerator roomGimmickGenerator, ItemKind[] itemKinds, UseItemMissionData.UseItemMissionStruct useItemMissionData)
        {
            _gameEventManager = gameEventManager;
            _roomGimmickGenerator = roomGimmickGenerator;
            _itemKinds = itemKinds;
            MissionName = useItemMissionData._missionName;
            MissionType = useItemMissionData._missionType;
            _targetItemID = useItemMissionData._targetItemID;
            _currentCompleteCount = 0;
            _targetCount = useItemMissionData._targetCompleteCount;
            _addEnemyPrefab = useItemMissionData._addEnemyPrefab;
            _addEnemyCount = useItemMissionData._addEnemyCount;
        }

        public void StartTracking()
        {
            _gameEventManager.OnGimmickCompleted += OnGimmickCompleted;
            AddItemForPlayer();
        }

        public void StopTracking()
        {
            _gameEventManager.OnGimmickCompleted -= OnGimmickCompleted;
        }

        private void OnGimmickCompleted(int useItemID)
        {
            if (useItemID != _targetItemID) return;

            _currentCompleteCount++;
            Debug.Log($"ギミックミッション進捗: {_currentCompleteCount}/{_targetCount}");

            if (_currentCompleteCount >= _targetCount)
            {
                OnMissionCompleted?.Invoke();
            }
        }
        
        private void AddItemForPlayer()
        {
            // プレイヤーにアイテムを付与
        }

        private void GenerateAddEnemy()
        {
            // 敵の生成
        }

    }
}

