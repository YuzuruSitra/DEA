using System.Collections.Generic;
using Gimmick;
using Manager;
using Mission.CreateScriptableObject;
using UnityEngine;

namespace Mission.Condition
{
    public class GimmickMissionCondition : IMissionCondition
    {
        public event System.Action OnMissionCompleted;
        
        public string MissionName { get; }
        public ClassType ClassType { get; }
        public MissionType MissionType { get; }
        private readonly RoomGimmickGenerator _roomGimmickGenerator;
        private readonly int _targetGimmickID;
        private readonly GimmickMissionData.GenerateType _generateType;
        private readonly GameObject[] _gimmickPrefab;
        private readonly GameObject[] _addGimmickPrefab;
        private readonly int _addGimmickCount;
        private readonly List<GameObject> _addGimmickList = new ();
        public string[] MissionLaunchLog { get; }
        public string[] MissionFinishLog { get; }
        
        public int CurrentCount { get; private set; }
        public int MaxCount { get; private set; }
        public GameObject StandOutTarget { get; private set; }
        
        public GimmickMissionCondition(RoomGimmickGenerator roomGimmickGenerator, GameObject[] gimmickPrefab, GimmickMissionData.GimmickMissionStruct gimmickMissionStruct)
        {
            _roomGimmickGenerator = roomGimmickGenerator;
            _gimmickPrefab = gimmickPrefab;
            MissionName = gimmickMissionStruct._missionName;
            ClassType = gimmickMissionStruct._classType;
            MissionType = gimmickMissionStruct._missionType;
            _targetGimmickID = gimmickMissionStruct._targetGimmickID;
            _generateType = gimmickMissionStruct._generateType;
            MaxCount = gimmickMissionStruct._targetCompleteCount;
            _addGimmickPrefab = gimmickMissionStruct._addGimmickPrefab;
            _addGimmickCount = gimmickMissionStruct._addGimmickCount;
            MissionLaunchLog = gimmickMissionStruct._missionLaunchLog;
            MissionFinishLog = gimmickMissionStruct._missionFinishLog;
        }

        public void StartTracking()
        {
            CurrentCount = 0;
            StandOutTarget = null;
            GenerateGimmick();
            GenerateAddGimmick();
        }

        public void StopTracking()
        {
            _roomGimmickGenerator.OnDestroyList(_addGimmickList);
        }
        
        public void PlayerChangeRoomEvent()
        {
            
        }

        public void OnDefeated(int id)
        {
            if (id != _targetGimmickID) return;
            CurrentCount++;

            if (CurrentCount >= MaxCount)
            {
                OnMissionCompleted?.Invoke();
            }
        }
        
        private void GenerateGimmick()
        {
            var actualCount = 0;
            for (var i = 0; i < MaxCount; i++)
            {
                var target = GetTargetGimmick();
                var targetRoom = GetTargetRoom();
                var insGimmick = _roomGimmickGenerator.InsGimmick(targetRoom, target);
                if (insGimmick != null) actualCount++;
                if (StandOutTarget == null) StandOutTarget = insGimmick;
            }
            MaxCount = actualCount;
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

