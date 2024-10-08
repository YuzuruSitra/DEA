using UnityEngine;

namespace Manager.Map
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
                var topLeftZ = roomInfo[i, (int)StageGenerator.RoomStatus.TopLeftZ];
                var bottomLeftX = roomInfo[i, (int)StageGenerator.RoomStatus.BottomLeftX];
                var bottomLeftZ = roomInfo[i, (int)StageGenerator.RoomStatus.BottomLeftZ];
                var bottomRightX = roomInfo[i, (int)StageGenerator.RoomStatus.BottomRightX];

                var targetX = Mathf.FloorToInt(pos.x);
                var targetZ = Mathf.FloorToInt(pos.z);

                var isInRoom = targetX >= bottomLeftX && targetX <= bottomRightX &&
                               targetZ >= bottomLeftZ && targetZ <= topLeftZ;

                if (isInRoom) return i;
            }
            return RoadNum;
        }
    }
}
