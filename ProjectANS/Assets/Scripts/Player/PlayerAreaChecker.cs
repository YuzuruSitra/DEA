using System;
using System.Map;
using UnityEngine;

namespace Player
{
    public class PlayerAreaChecker : MonoBehaviour
    {
        [SerializeField] private StageGenerator _stageGenerator;
        private bool _isInRoom;
        public event Action<bool> OnPlayerRoomChanged; // イベントデリゲートの定義

        private void Update()
        {
            IsPlayerInRoom();
        }

        private void IsPlayerInRoom()
        {
            var roomInfo = _stageGenerator.RoomInfo;
            var previousInRoom = _isInRoom; // 前回の_isInRoomの値を保存

            for (var i = 0; i < _stageGenerator.RoomCount; i++)
            {
                // 部屋の四隅の座標を取得
                var topLeftZ = roomInfo[i, (int)StageGenerator.RoomStatus.TopLeftZ];
                var bottomLeftX = roomInfo[i, (int)StageGenerator.RoomStatus.BottomLeftX];
                var bottomLeftZ = roomInfo[i, (int)StageGenerator.RoomStatus.BottomLeftZ];
                var bottomRightX = roomInfo[i, (int)StageGenerator.RoomStatus.BottomRightX];

                // プレイヤーの座標を整数に変換して範囲内にあるか判定
                var pos = transform.position;
                var playerX = Mathf.FloorToInt(pos.x);
                var playerZ = Mathf.FloorToInt(pos.z);

                // 範囲内にあるかどうかの判定
                _isInRoom =
                    playerX >= bottomLeftX && playerX <= bottomRightX &&
                    playerZ >= bottomLeftZ && playerZ <= topLeftZ;

                if (_isInRoom) break;
            }

            // 前回の_isInRoomと異なる場合にイベントを発行
            if (previousInRoom != _isInRoom && OnPlayerRoomChanged != null)
            {
                OnPlayerRoomChanged.Invoke(_isInRoom);
            }
        }
    }
}