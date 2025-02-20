using System.Collections.Generic;
using Gimmick;
using Item;
using Manager;
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
        private readonly InventoryHandler _inventoryHandler;
        private readonly int _missionItemID;
        private readonly int _targetCount;
        private int _currentCompleteCount;
        private readonly GimmickMissionData.GenerateType _generateType;
        private readonly ItemKind[] _itemKinds;
        private readonly GameObject[] _addEnemyPrefab;
        private readonly int _addEnemyCount;
        private readonly List<GameObject> _addEnemyList = new ();
        
        public UseItemMissionCondition(GameEventManager gameEventManager, RoomGimmickGenerator roomGimmickGenerator, InventoryHandler inventoryHandler, ItemKind[] itemKinds, UseItemMissionData.UseItemMissionStruct useItemMissionData)
        {
            _gameEventManager = gameEventManager;
            _roomGimmickGenerator = roomGimmickGenerator;
            _inventoryHandler = inventoryHandler;
            _itemKinds = itemKinds;
            MissionName = useItemMissionData._missionName;
            MissionType = useItemMissionData._missionType;
            _missionItemID = useItemMissionData._missionItemID;
            _currentCompleteCount = 0;
            _targetCount = useItemMissionData._targetCompleteCount;
            _addEnemyPrefab = useItemMissionData._addEnemyPrefab;
            _addEnemyCount = useItemMissionData._addEnemyCount;
        }

        public void StartTracking()
        {
            _gameEventManager.OnItemUsed += OnGimmickCompleted;
            AddItemForPlayer();
            GenerateAddEnemy();
        }

        public void StopTracking()
        {
            _roomGimmickGenerator.OnDestroyList(_addEnemyList);
            _gameEventManager.OnItemUsed -= OnGimmickCompleted;
        }

        private void OnGimmickCompleted(int itemMissionID)
        {
            if (itemMissionID != _missionItemID) return;

            _currentCompleteCount++;
            Debug.Log($"アイテムミッション進捗: {_currentCompleteCount}/{_targetCount}");

            if (_currentCompleteCount >= _targetCount)
            {
                OnMissionCompleted?.Invoke();
            }
        }
        
        private void AddItemForPlayer()
        {
            // プレイヤーにアイテムを付与
            _inventoryHandler.AddItem(_itemKinds[_missionItemID]);
        }

        private void GenerateAddEnemy()
        {
            // 敵の生成
            for (var i = 0; i < _addEnemyCount; i++)
            {
                var target = _addEnemyPrefab[Random.Range(0, _addEnemyPrefab.Length)];
                var insAddObj = _roomGimmickGenerator.InsGimmick(_roomGimmickGenerator.GetRandomRoom, target);
                _addEnemyList.Add(insAddObj);
            }
        }

    }
}

