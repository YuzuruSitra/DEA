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
            Explorer
        }
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
        [SerializeField] private bool _isDebug;
        private readonly Dictionary<PlayerType, int> _points = new()
        {
            { PlayerType.Killer, 0 },
            { PlayerType.Achiever, 0 },
            { PlayerType.Explorer, 0 }
        };
        
        private void Start()
        {
            _playerTypeClassifier = new PlayerTypeClassifier(_logPerSend);
            _playerTypeClassifier.ResponsePlayerType += ReceivePlayerType;
        }

        private void OnDestroy()
        {
            _playerTypeClassifier.ResponsePlayerType -= ReceivePlayerType;
        }

        private void Update()
        {
            // デバッグ用
            if (!_isDebug) return;
            var killer = 6;
            var achiever = 6;
            var explorer = 6;
            
            if (Input.GetKeyDown(KeyCode.A)) killer = 10;
            if (Input.GetKeyDown(KeyCode.S)) achiever = 10;
            if (Input.GetKeyDown(KeyCode.D)) explorer = 10;
            
            if (Input.anyKeyDown) _playerTypeClassifier.CollectActionLog(killer, achiever, explorer);
        }

        private void OnApplicationQuit()
        {
            _playerTypeClassifier.OnDisconnected();
        }
        
        public void SendLogsForMetaAI(AddScores[] scores)
        {
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
        
    }
}
