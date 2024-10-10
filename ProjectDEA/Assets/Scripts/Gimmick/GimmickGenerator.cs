using UnityEngine;
using System;
using Manager.Map;
using System.Collections.Generic;

namespace Gimmick
{
    public class GimmickGenerator : MonoBehaviour
    {
        private const int PaddingThreshold = 1;
        public enum GimmickKind
        {
            Water,
            ExitHole,
            TreasureBox
        }
        [Serializable]
        public struct GimmickInfo
        {
            public GimmickKind _kind;
            public GameObject _prefab;
            public bool _isRoomGenerate;
            public bool _isJustOne;
        }
        [SerializeField] private GimmickInfo[] _gimmickInfo;
        [SerializeField] private Transform _mapParent;

        public void GenerateGimmick(StageGenerator stageGenerator)
        {
            var insList = new List<GimmickInfo>();
            foreach (var gimmick in _gimmickInfo)
            {
                if (gimmick._isRoomGenerate)
                {
                    insList.Add(gimmick);
                }
            }

            
            var roomCount = stageGenerator.RoomCount;
            var groundY = stageGenerator.GroundPosY;
            var roomInfo = stageGenerator.RoomInfo;

            for (var i = 0; i < roomCount - 1; i++)
            {
                var gimmickNum = UnityEngine.Random.Range(0, insList.Count);
                var gimmickInfo = insList[gimmickNum];
                var insObj = gimmickInfo._prefab;
                if (gimmickInfo._isJustOne) insList.RemoveAt(gimmickNum);
                InsGimmick(groundY, roomInfo, insObj, i);
            }
            // 出口の設置
            InsGimmick(groundY, roomInfo, _gimmickInfo[(int)GimmickKind.ExitHole]._prefab, roomCount - 1);
        }

        private static Vector3 SetVector3(int x, float y, int z)
        {
            var insPos = Vector3.zero;
            insPos.x = x;
            insPos.y = y;
            insPos.z = z;
            return insPos;
        }

        private void InsGimmick(float groundY, int[,] roomInfo, GameObject insObj,int roomNum)
        {
            var paddingX = (int)Math.Ceiling(insObj.transform.localScale.x / 2.0f) + PaddingThreshold;
            var paddingZ =  (int)Math.Ceiling(insObj.transform.localScale.z / 2.0f) + PaddingThreshold;
            var rangeMinX = roomInfo[roomNum, (int)StageGenerator.RoomStatus.TopLeftX] + paddingX;
            var rangeMaxX = roomInfo[roomNum, (int)StageGenerator.RoomStatus.TopRightX] - paddingX;
            var insPosX = UnityEngine.Random.Range(rangeMinX, rangeMaxX + 1);
            var rangeMinZ = roomInfo[roomNum, (int)StageGenerator.RoomStatus.BottomLeftZ] + paddingZ;
            var rangeMaxZ = roomInfo[roomNum, (int)StageGenerator.RoomStatus.TopLeftZ ] - paddingZ;
            var insPosZ = UnityEngine.Random.Range(rangeMinZ, rangeMaxZ + 1);
            var insPosY = groundY + insObj.transform.localScale.y / 2.0f;
            var insPos = SetVector3(insPosX, insPosY, insPosZ);
            Instantiate(insObj, insPos, insObj.transform.rotation, _mapParent);
        }

    }
}