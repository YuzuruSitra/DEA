using System.Map;
using UnityEngine;

namespace Player
{
    public class InRoomChecker
    {
        private readonly StageGenerator _stageGenerator = GameObject.FindWithTag("StageGenerator").GetComponent<StageGenerator>();
        public const int RoadNum = -1;
        public int CheckStayRoomNum(Vector3 pos)
        {
            var roomInfo = _stageGenerator.RoomInfo;

            for (var i = 0; i < _stageGenerator.RoomCount; i++)
            {
                // 部屋の四隅の座標を取得
                var topLeftZ = roomInfo[i, (int)StageGenerator.RoomStatus.TopLeftZ];
                var bottomLeftX = roomInfo[i, (int)StageGenerator.RoomStatus.BottomLeftX];
                var bottomLeftZ = roomInfo[i, (int)StageGenerator.RoomStatus.BottomLeftZ];
                var bottomRightX = roomInfo[i, (int)StageGenerator.RoomStatus.BottomRightX];

                // プレイヤーの座標を整数に変換して範囲内にあるか判定
                var playerX = Mathf.FloorToInt(pos.x);
                var playerZ = Mathf.FloorToInt(pos.z);

                // 範囲内にあるかどうかの判定
                var isInRoom = playerX >= bottomLeftX && playerX <= bottomRightX &&
                               playerZ >= bottomLeftZ && playerZ <= topLeftZ;

                if (isInRoom) return i;
            }
            return RoadNum;
        }
    }
}
