using UnityEngine;
using System;
using Manager.Map;
using System.Collections.Generic;
using System.Linq;

namespace Gimmick
{
    public class GimmickGenerator : MonoBehaviour
    {
        private const int PaddingThreshold = 1;
        public enum GimmickKind
        {
            ExitObelisk,
            ObeliskKeyOut,
            TreasureBox,
            EnemySpawnArea,
            BornOut
        }

        [Serializable]
        public struct GimmickInfo
        {
            public GimmickKind _kind;
            public GameObject _prefab;
            public bool _isRoomGenerate;
            public int _minCount; // 0 no limit
            public int _maxCount; // 0 no limit
        }
        
        private const int LimitValue = 0;
        private struct PlacedGimmickInfo
        {
            public Vector3 InsMinPos;
            public Vector3 InsMaxPos;
        }
        
        [SerializeField] private GimmickInfo[] _gimmickInfo;
        private int[] _gimmickInsCount;
        [SerializeField] private int _maxGimmickPerRoom;
        [SerializeField] private int _minGimmickPerRoom;
        [SerializeField] private Transform _mapParent;
        // 絆創膏
        private int[] _obeliskKeyRooms = new int[4];
        private int _insKeyCount;
        
        public void GenerateGimmick(StageGenerator stageGenerator)
        {
            var roomCount = stageGenerator.RoomCount;
            var groundY = stageGenerator.GroundPosY;
            var roomInfo = stageGenerator.RoomInfo;
            _gimmickInsCount = new int[_gimmickInfo.Length];
            var exitObeliskRoom = CalcMostBigRoom(roomCount, roomInfo);
            
            _obeliskKeyRooms = Enumerable.Range(0, stageGenerator.RoomCount).OrderBy(x => UnityEngine.Random.Range(0, stageGenerator.RoomCount)).Take(4).ToArray();
            Debug.Log(string.Join(", ", _obeliskKeyRooms));
            
            // 各部屋のギミック生成数を事前に決定
            var roomGimmickCount = new int[roomCount];
            for (var i = 0; i < roomCount; i++)
            {
                roomGimmickCount[i] = UnityEngine.Random.Range(_minGimmickPerRoom, _maxGimmickPerRoom + 1);
                if (i == exitObeliskRoom) roomGimmickCount[i]--;
            }
            
            // 生成必須対象のリスト作成
            var neededInsList = new List<GimmickInfo>();
            foreach (var target in _gimmickInfo)
            {
                if (!target._isRoomGenerate) continue;
                if (target._minCount == LimitValue) continue;
                neededInsList.AddRange(Enumerable.Repeat(target, target._minCount));
                _gimmickInsCount[(int)target._kind] = target._minCount;
            }
            
            // 生成必須対象の部屋割り当て
            var neededInsTargetRoom = new List<int>();
            var availableRoomIndexes = new List<int>();

            // 各部屋に生成必須ギミックを割り当てるためのリストを準備
            for (var i = 0; i < roomCount; i++)
            {
                for (var j = 0; j < roomGimmickCount[i]; j++)
                {
                    availableRoomIndexes.Add(i);
                }
            }

            // 生成必須ギミックの部屋割り当て
            for (var i = 0; i < neededInsList.Count(); i++)
            {
                // 割り当て可能な部屋がなくなった場合は終了
                if (availableRoomIndexes.Count == 0) break;

                var targetNum = UnityEngine.Random.Range(0, availableRoomIndexes.Count);
                neededInsTargetRoom.Add(availableRoomIndexes[targetNum]);
                availableRoomIndexes.RemoveAt(targetNum);
            }
            
            // 生成対象のリスト作成
            var insList = new List<GimmickInfo>();
            foreach (var target in _gimmickInfo)
            {
                if (!target._isRoomGenerate) continue;
                if (target._maxCount != LimitValue && target._maxCount <= _gimmickInsCount[(int)target._kind]) continue;
                insList.Add(target);
            }

            for (var i = 0; i < roomCount; i++)
            {
                var isInsKey = false;
                // Create a list of gimmick coordinates for each room
                var placedGimmickInfo = new List<PlacedGimmickInfo>();
                // Generate Exit
                if (i == exitObeliskRoom)
                    InsGimmick(groundY, roomInfo, i, _gimmickInfo[(int)GimmickKind.ExitObelisk],
                        placedGimmickInfo);
                
                for (var j = 0; j < roomGimmickCount[i]; j++)
                {
                    if (insList.Count == 0) break;
                    var gimmickInfo = insList[UnityEngine.Random.Range(0, insList.Count)];
                    // 生成必須対象の生成部屋か確認しマッチした場合はセット
                    var list = neededInsTargetRoom;
                    for (var index = 0; index < list.Count(); index++)
                    {
                        var targetRoom = neededInsTargetRoom[index];
                        if (i != targetRoom) continue;
                        gimmickInfo = neededInsList[index];
                        neededInsTargetRoom.RemoveAt(index);
                    }
                    
                    // 絆創膏
                    if (_insKeyCount < _obeliskKeyRooms.Length  && !isInsKey)
                    {
                        foreach (var variable in _obeliskKeyRooms)
                        {
                            if (variable != i) continue;
                            gimmickInfo = _gimmickInfo[(int)GimmickKind.ObeliskKeyOut];
                            isInsKey = true;
                            Debug.Log("a");
                            _insKeyCount++;
                        }
                    }

                    var targetNum = (int)gimmickInfo._kind;
                    _gimmickInsCount[targetNum]++;

                    // Gimmicks that can generate only one are removed from the list
                    if (gimmickInfo._maxCount != LimitValue && gimmickInfo._maxCount <= _gimmickInsCount[targetNum])
                    {
                        // Use Find to directly locate the item to remove
                        var itemToRemove = insList.Find(item => item._kind == gimmickInfo._kind);
                        if (itemToRemove._kind == gimmickInfo._kind)
                        {
                            insList.Remove(itemToRemove);
                        }
                    }
                    
                    // Generate gimmicks and register them in the coordinate list
                    InsGimmick(groundY, roomInfo, i, gimmickInfo, placedGimmickInfo);
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

        private void InsGimmick(float groundY, int[,] roomInfo, int roomNum, GimmickInfo gimmickInfo, List<PlacedGimmickInfo> placedGimmickList)
        {
            var insObj = gimmickInfo._prefab;
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

        private static Vector3 CalcDiffPos(Vector3 pos, float valueX, float valueZ)
        {
            pos.x += valueX;
            pos.z += valueZ;
            return pos;
        }

        private static int CalcMostBigRoom(int roomCount, int[,] roomInfo)
        {
            var mostBigRoom = 0;
            var currentSize = 0;
            for (var i = 0; i < roomCount; i++)
            {
                var size = roomInfo[i, (int)StageGenerator.RoomStatus.Rw] *
                           roomInfo[i, (int)StageGenerator.RoomStatus.Rh];
                if (currentSize >= size) continue;
                mostBigRoom = i;
                currentSize = size;
            }
            return mostBigRoom;
        }
        
    }
}