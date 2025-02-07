using UnityEngine;

namespace Mission.Condition
{
    public class GimmickMissionCondition : IMissionCondition
    {
        public event System.Action OnMissionCompleted;
        
        public string MissionName { get; }
        public MissionType MissionType { get; }
        private readonly GameEventManager _gameEventManager;
        private readonly int _targetGimmickID;
        private readonly int _targetCount;
        private int _currentCount;

        public GimmickMissionCondition(GameEventManager gameEventManager, string missionName, int gimmickID, int count)
        {
            MissionName = missionName;
            _gameEventManager = gameEventManager;
            _targetGimmickID = gimmickID;
            _currentCount = 0;
            _targetCount = count;
        }

        public void StartTracking()
        {
            _gameEventManager.OnGimmickCompleted += OnGimmickCompleted;
        }

        public void StopTracking()
        {
            _gameEventManager.OnGimmickCompleted -= OnGimmickCompleted;
        }

        private void OnGimmickCompleted(int gimmickID)
        {
            if (gimmickID != _targetGimmickID) return;

            _currentCount++;
            Debug.Log($"ギミックミッション進捗: {_currentCount}/{_targetCount}");

            if (_currentCount >= _targetCount)
            {
                OnMissionCompleted?.Invoke();
            }
        }
    }
}

