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

        private struct PlacedGimmickInfo
        {
            public Vector3 InsMinPos;
            public Vector3 InsMaxPos;
        }
        
        [SerializeField] private GimmickInfo[] _gimmickInfo;
        [SerializeField] private int _maxGimmickPerRoom;
        [SerializeField] private Transform _mapParent;
        
        public void GenerateGimmick(StageGenerator stageGenerator)
        {
            var insList = new List<GimmickInfo>();
            foreach (var gimmick in _gimmickInfo)
                if (gimmick._isRoomGenerate)
                    insList.Add(gimmick);

            var roomCount = stageGenerator.RoomCount;
            var groundY = stageGenerator.GroundPosY;
            var roomInfo = stageGenerator.RoomInfo;

            for (var i = 0; i < roomCount; i++)
            {
                // Determine the number of gimmicks produced
                var gimmickCount = UnityEngine.Random.Range(1, _maxGimmickPerRoom + 1);
                // Create a list of gimmick coordinates for each room
                var placedGimmickInfo = new List<PlacedGimmickInfo>();
                // Generate Exit
                if (i == roomCount - 1)
                {
                    gimmickCount--;
                    InsGimmick(groundY, roomInfo, i, _gimmickInfo[(int)GimmickKind.ExitHole]._prefab, placedGimmickInfo);
                }
                
                for (var j = 0; j < gimmickCount; j++)
                {
                    if (insList.Count == 0) break;
                    var gimmickNum = UnityEngine.Random.Range(0, insList.Count);
                    var gimmickInfo = insList[gimmickNum];
                    
                    // Gimmicks that can generate only one are removed from the list
                    if (gimmickInfo._isJustOne) insList.RemoveAt(gimmickNum);

                    // Generate gimmicks and register them in the coordinate list
                    InsGimmick(groundY, roomInfo, i, gimmickInfo._prefab, placedGimmickInfo);
                }
            }
        }

        private static Vector3 SetVector3(int x, float y, int z)
        {
            var insPos = Vector3.zero;
            insPos.x = x;
            insPos.y = y;
            insPos.z = z;
            return insPos;
        }

        private void InsGimmick(float groundY, int[,] roomInfo, int roomNum, GameObject insObj, List<PlacedGimmickInfo> placedGimmickList)
        {
            var halfScaleX = insObj.transform.localScale.x / 2.0f;
            var halfScaleZ = insObj.transform.localScale.z / 2.0f;
            var paddingX = (int)Math.Ceiling(halfScaleX) + PaddingThreshold;
            var paddingZ = (int)Math.Ceiling(halfScaleZ) + PaddingThreshold;
            var rangeMinX = roomInfo[roomNum, (int)StageGenerator.RoomStatus.TopLeftX] + paddingX;
            var rangeMaxX = roomInfo[roomNum, (int)StageGenerator.RoomStatus.TopRightX] - paddingX;
            var rangeMinZ = roomInfo[roomNum, (int)StageGenerator.RoomStatus.BottomLeftZ] + paddingZ;
            var rangeMaxZ = roomInfo[roomNum, (int)StageGenerator.RoomStatus.TopLeftZ] - paddingZ;

            var validPositions = new List<Vector3>();

            // Generate list of available coordinates
            var insPosY = groundY + insObj.transform.localScale.y / 2.0f;
            for (var x = rangeMinX; x < rangeMaxX; x++)
            {
                for (var z = rangeMinZ; z <= rangeMaxZ; z++)
                {
                    var insPos = SetVector3(x, insPosY, z);
                    validPositions.Add(insPos);
                }
            }

            // Process to eliminate duplication of gimmicks
            var carefulValidPos = new List<Vector3>();
            foreach (var t in validPositions)
            {
                var minPos = CalcDiffPos(t, -halfScaleX, -halfScaleZ);
                var maxPos = CalcDiffPos(t, halfScaleX, halfScaleZ);
                
                // Overlap flags
                var overlapFound = false;
                foreach (var placedInfo in placedGimmickList)
                {
                    if (minPos.x - PaddingThreshold >= placedInfo.InsMaxPos.x ||
                        maxPos.x + PaddingThreshold <= placedInfo.InsMinPos.x ||
                        minPos.z - PaddingThreshold >= placedInfo.InsMaxPos.z ||
                        maxPos.z + PaddingThreshold <= placedInfo.InsMinPos.z) continue;
                    overlapFound = true;
                    break;
                }

                if (!overlapFound) carefulValidPos.Add(t);
            }
            
            // Randomly selected coordinates
            if (carefulValidPos.Count <= 0) return;
            var randomIndex = UnityEngine.Random.Range(0, carefulValidPos.Count);
            var careInsPos = carefulValidPos[randomIndex];
            
            var info = new PlacedGimmickInfo
            {
                InsMinPos = CalcDiffPos(careInsPos, -halfScaleX, -halfScaleZ),
                InsMaxPos = CalcDiffPos(careInsPos, halfScaleX, halfScaleZ)
            };
            // Add Position list
            placedGimmickList.Add(info);

            Instantiate(insObj, careInsPos, insObj.transform.rotation, _mapParent);
        }

        private Vector3 CalcDiffPos(Vector3 pos, float valueX, float valueZ)
        {
            pos.x += valueX;
            pos.z += valueZ;
            return pos;
        }
    }
}
