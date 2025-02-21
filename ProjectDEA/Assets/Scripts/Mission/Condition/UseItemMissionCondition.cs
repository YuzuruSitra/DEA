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
        private readonly GimmickMissionData.GenerateType _generateType;
        private readonly ItemKind[] _itemKinds;
        private readonly GameObject[] _addEnemyPrefab;
        private readonly int _addEnemyCount;
        private readonly List<GameObject> _addEnemyList = new ();
        public string[] MissionLaunchLog { get; }
        public string[] MissionFinishLog { get; }
        
        public int CurrentCount { get; private set; }
        public int MaxCount { get; }
        public GameObject StandOutTarget { get; private set; }
        
        public UseItemMissionCondition(GameEventManager gameEventManager, RoomGimmickGenerator roomGimmickGenerator, InventoryHandler inventoryHandler, ItemKind[] itemKinds, UseItemMissionData.UseItemMissionStruct useItemMissionData)
        {
            _gameEventManager = gameEventManager;
            _roomGimmickGenerator = roomGimmickGenerator;
            _inventoryHandler = inventoryHandler;
            _itemKinds = itemKinds;
            MissionName = useItemMissionData._missionName;
            MissionType = useItemMissionData._missionType;
            _missionItemID = useItemMissionData._missionItemID;
            MaxCount = useItemMissionData._targetCompleteCount;
            _addEnemyPrefab = useItemMissionData._addEnemyPrefab;
            _addEnemyCount = useItemMissionData._addEnemyCount;
            MissionLaunchLog = useItemMissionData._missionLaunchLog;
            MissionFinishLog = useItemMissionData._missionFinishLog;
        }

        public void StartTracking()
        {
            CurrentCount = 0;
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

            CurrentCount++;

            if (CurrentCount >= MaxCount)
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

