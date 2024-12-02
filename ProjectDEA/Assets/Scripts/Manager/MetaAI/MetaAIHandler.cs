using System;
using System.Collections.Generic;
using UnityEngine;

namespace Manager.MetaAI
{
    public class MetaAIHandler : MonoBehaviour
    {
        public enum PlayerType
        {
            Killer,
            Achiever,
            Explorer,
            None
        }
        public readonly int PlayerTypeCount = Enum.GetValues(typeof(PlayerType)).Length - 1;
        public PlayerType CurrentPlayerType { get; private set; }
        
        [Serializable]
        public struct AddScores
        {
            public PlayerType _type;
            public int _point;
        }
        
        private PlayerTypeClassifier _playerTypeClassifier;
        [Header("一度に送るログ数")]
        [SerializeField] private int _logPerSend = 10;
        private bool _isUse;
        [SerializeField] private bool _isDebugInput;
        private readonly Dictionary<PlayerType, int> _points = new()
        {
            { PlayerType.Killer, 0 },
            { PlayerType.Achiever, 0 },
            { PlayerType.Explorer, 0 }
        };
        public event Action OnAddEvent;
        private bool _addedListener;
        
        private void Awake()
        {
            CheckSingleton();
        }

        public void LaunchMetaAI()
        {
            if (!_isUse) return;
            _playerTypeClassifier.ConnectToPythonServer();
        }
        
        public void ResetMetaAI()
        {
            _playerTypeClassifier.SendResetToPython();
        }

        private void CheckSingleton()
        {
            var target = GameObject.FindGameObjectWithTag(gameObject.tag);
            var checkResult = target != null && target != gameObject;

            if (checkResult)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            
            CurrentPlayerType = PlayerType.None;
            if (!_isUse) return;
            _playerTypeClassifier = new PlayerTypeClassifier(_logPerSend);
            _playerTypeClassifier.ResponsePlayerType += ReceivePlayerType;
            _addedListener = true;
        }

        private void OnDestroy()
        {
            if (!_isUse) return;
            if (!_addedListener) return;
            _playerTypeClassifier.ResponsePlayerType -= ReceivePlayerType;
        }
        
        private void OnApplicationQuit()
        {
            if (!_isUse) return;
            _playerTypeClassifier.OnDisconnected();
        }
        
        public void SendLogsForMetaAI(AddScores[] scores)
        {
            if (!_isUse) return;
            OnAddEvent?.Invoke();
            // ポイントを初期化
            _points[PlayerType.Killer] = 0;
            _points[PlayerType.Achiever] = 0;
            _points[PlayerType.Explorer] = 0;

            foreach (var score in scores)
            {
                if (_points.ContainsKey(score._type))
                {
                    _points[score._type] += score._point;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(score._type), score._type, "Unsupported player type.");
                }
            }

            _playerTypeClassifier.CollectActionLog(
                _points[PlayerType.Killer],
                _points[PlayerType.Achiever],
                _points[PlayerType.Explorer]
            );
        }

        private void ReceivePlayerType(PlayerType newType)
        {
            CurrentPlayerType = newType;
        }

        public bool ChangeUseBool()
        {
            _isUse = !_isUse;
            return _isUse;
        }
        
    }
}
