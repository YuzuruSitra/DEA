using System.Collections.Generic;
using Gimmick;
using Mission.CreateScriptableObject;
using UnityEngine;

namespace Mission.Condition
{
    public class GimmickMissionCondition : IMissionCondition
    {
        public event System.Action OnMissionCompleted;
        
        public string MissionName { get; }
        public MissionType MissionType { get; }
        private readonly GameEventManager _gameEventManager;
        private readonly RoomGimmickGenerator _roomGimmickGenerator;
        private readonly int _targetGimmickID;
        private readonly GimmickMissionData.GenerateType _generateType;
        private readonly GameObject[] _gimmickPrefab;
        private readonly GameObject[] _addGimmickPrefab;
        private readonly int _addGimmickCount;
        private readonly List<GameObject> _addGimmickList = new ();
        public int CurrentCount { get; private set; }
        public int MaxCount { get; }
        
        public GimmickMissionCondition(GameEventManager gameEventManager, RoomGimmickGenerator roomGimmickGenerator, GameObject[] gimmickPrefab, GimmickMissionData.GimmickMissionStruct gimmickMissionStruct)
        {
            _gameEventManager = gameEventManager;
            _roomGimmickGenerator = roomGimmickGenerator;
            _gimmickPrefab = gimmickPrefab;
            MissionName = gimmickMissionStruct._missionName;
            MissionType = gimmickMissionStruct._missionType;
            _targetGimmickID = gimmickMissionStruct._targetGimmickID;
            CurrentCount = 0;
            _generateType = gimmickMissionStruct._generateType;
            MaxCount = gimmickMissionStruct._targetCompleteCount;
            _addGimmickPrefab = gimmickMissionStruct._addGimmickPrefab;
            _addGimmickCount = gimmickMissionStruct._addGimmickCount;
        }

        public void StartTracking()
        {
            _gameEventManager.OnGimmickCompleted += OnGimmickCompleted;
            GenerateGimmick();
            GenerateAddGimmick();
        }

        public void StopTracking()
        {
            _roomGimmickGenerator.OnDestroyList(_addGimmickList);
            _gameEventManager.OnGimmickCompleted -= OnGimmickCompleted;
        }

        private void OnGimmickCompleted(int gimmickID)
        {
            if (gimmickID != _targetGimmickID) return;

            CurrentCount++;
            Debug.Log($"ギミックミッション進捗: {CurrentCount}/{MaxCount}");

            if (CurrentCount >= MaxCount)
            {
                OnMissionCompleted?.Invoke();
            }
        }
        
        private void GenerateGimmick()
        {
            for (var i = 0; i < MaxCount; i++)
            {
                var target = GetTargetGimmick();
                var targetRoom = GetTargetRoom();
                _roomGimmickGenerator.InsGimmick(targetRoom, target);
                Debug.Log("generate : " + target.name + " Room : " + targetRoom);
            }
        }

        private void GenerateAddGimmick()
        {
            for (var i = 0; i < _addGimmickCount; i++)
            {
                var target = _addGimmickPrefab[Random.Range(0, _addGimmickPrefab.Length)];
                var insAddObj = _roomGimmickGenerator.InsGimmick(_roomGimmickGenerator.GetRandomRoom, target);
                _addGimmickList.Add(insAddObj);
            }
        }
        
        private GameObject GetTargetGimmick()
        {
            return _targetGimmickID switch
            {
                GimmickMissionData.NonTargetID => _gimmickPrefab[Random.Range(0, _gimmickPrefab.Length)],
                _ => _gimmickPrefab[_targetGimmickID]
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

