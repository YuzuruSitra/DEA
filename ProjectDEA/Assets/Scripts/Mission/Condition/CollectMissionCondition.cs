using UnityEngine;

namespace Mission.Condition
{
    public class CollectMissionCondition : IMissionCondition
    {
        public event System.Action OnMissionCompleted;
        
        public string MissionName { get; set; }
        public MissionType MissionType { get; }
        private readonly GameEventManager _gameEventManager;
        private readonly int _targetItemID;
        private readonly int _targetCount;
        private int _currentCount;

        public CollectMissionCondition(GameEventManager gameEventManager, string missionName, int itemID, int count)
        {
            MissionName = missionName;
            _gameEventManager = gameEventManager;
            _targetItemID = itemID;
            _currentCount = 0;
            _targetCount = count;
        }

        public void StartTracking()
        {
            _gameEventManager.OnItemCollected += OnItemCollected;
        }

        public void StopTracking()
        {
            _gameEventManager.OnItemCollected -= OnItemCollected;
        }

        private void OnItemCollected(int itemID)
        {
            if (itemID != _targetItemID) return;

            _currentCount++;
            Debug.Log($"アイテム収集ミッション進捗: {_currentCount}/{_targetCount}");

            if (_currentCount >= _targetCount)
            {
                OnMissionCompleted?.Invoke();
            }
        }
    }
}
