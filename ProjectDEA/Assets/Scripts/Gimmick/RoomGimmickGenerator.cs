using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Manager.Map;

namespace Gimmick
{
    public class RoomGimmickGenerator : MonoBehaviour
    {
        private const int PaddingThreshold = 1;
        private const int LimitValue = 0;

        public enum GimmickKind
        {
            ExitObelisk,
            ObeliskKeyOut,
            TreasureBox,
            EnemySpawnArea,
            BornOut,
            Monument
        }

        [Serializable]
        public struct GimmickInfo
        {
            public GimmickKind _kind;
            public GameObject _prefab;
            public bool _isRoomGenerate;
            public int _minCount;
            public int _maxCount;
        }

        private struct PlacedGimmickInfo
        {
            public Vector3 InsMinPos;
            public Vector3 InsMaxPos;
        }

        [SerializeField] private GimmickInfo[] _gimmickInfo;
        public GimmickInfo[] GimmickInfos => _gimmickInfo;
        [SerializeField] private int _maxGimmickPerRoom;
        [SerializeField] private int _minGimmickPerRoom;
        [SerializeField] private Transform _mapParent;
        [SerializeField] private int _insKeyCount;
        private int[] _gimmickInsCount;
        
        private Dictionary<int, List<(Vector3 minPos, Vector3 maxPos)>> _inRoomGimmickPos = new();

        public void GenerateGimmicks(StageGenerator stageGenerator)
        {
            var roomCount = stageGenerator.RoomCount;
            var groundY = stageGenerator.GroundPosY;
            var roomInfo = stageGenerator.RoomInfo;
            
            // roomCount分の初期化
            for (var i = 0; i < roomCount; i++)
            {
                _inRoomGimmickPos[i] = new List<(Vector3, Vector3)>();
            }
            
            // foreach (var room in _inRoomGimmickPos)
            // {
            //     Debug.Log($"Room {room.Key} has {room.Value[0].maxPos} elements.");
            // }
            
            // ギミックの初期化
            _gimmickInsCount = new int[_gimmickInfo.Length];
            
            // ExitObeliskを生成
            var exitObeliskRoom = CalcMostBigRoom(roomCount, roomInfo);
            GenerateExitObelisk(exitObeliskRoom, groundY, roomInfo);

            // ObeliskKeyOutを生成
            GenerateObeliskKeys(stageGenerator, groundY, roomInfo);

            // 特定条件に応じたギミックを生成
            GenerateRoomSpecificGimmicks(stageGenerator, groundY, roomInfo, exitObeliskRoom);
        }

        private void GenerateExitObelisk(int roomIndex, float groundY, int[,] roomInfo)
        {
            var placedGimmickInfo = new List<PlacedGimmickInfo>();
            InsGimmick(groundY, roomInfo, roomIndex, _gimmickInfo[(int)GimmickKind.ExitObelisk], placedGimmickInfo);
        }

        private void GenerateObeliskKeys(StageGenerator stageGenerator, float groundY, int[,] roomInfo)
        {
            var obeliskKeyRooms = Enumerable.Range(0, stageGenerator.RoomCount)
                                         .OrderBy(_ => UnityEngine.Random.Range(0, stageGenerator.RoomCount))
                                         .Take(_insKeyCount)
                                         .ToArray();

            foreach (var roomIndex in obeliskKeyRooms)
            {
                var placedGimmickInfo = new List<PlacedGimmickInfo>();
                InsGimmick(groundY, roomInfo, roomIndex, _gimmickInfo[(int)GimmickKind.ObeliskKeyOut], placedGimmickInfo);
            }
        }

        private void GenerateRoomSpecificGimmicks(StageGenerator stageGenerator, float groundY, int[,] roomInfo, int exitObeliskRoom)
        {
            var roomCount = stageGenerator.RoomCount;
            var roomGimmickCount = new int[roomCount];

            // 各部屋のギミック生成数を事前に決定
            for (var i = 0; i < roomCount; i++)
            {
                roomGimmickCount[i] = UnityEngine.Random.Range(_minGimmickPerRoom, _maxGimmickPerRoom + 1);
                if (i == exitObeliskRoom) roomGimmickCount[i]--;
            }

            // 各部屋でギミックを生成
            for (var i = 0; i < roomCount; i++)
            {
                var placedGimmickInfo = new List<PlacedGimmickInfo>();
                for (var j = 0; j < roomGimmickCount[i]; j++)
                {
                    // 配置可能なギミックを取得
                    var validGimmicks = _gimmickInfo
                        .Where(g => g._isRoomGenerate && 
                                    (g._maxCount == LimitValue || g._maxCount > _gimmickInsCount[(int)g._kind]))
                        .ToList();

                    if (!validGimmicks.Any()) break;

                    // ランダムにギミックを選択
                    var gimmickInfo = validGimmicks[UnityEngine.Random.Range(0, validGimmicks.Count)];

                    // ギミックを生成
                    InsGimmick(groundY, roomInfo, i, gimmickInfo, placedGimmickInfo);

                    // 生成カウントを更新
                    _gimmickInsCount[(int)gimmickInfo._kind]++;
                }
            }
        }

        private void InsGimmick(float groundY, int[,] roomInfo, int roomNum, GimmickInfo gimmickInfo, List<PlacedGimmickInfo> placedGimmickList)
        {
            // 配置可能な範囲を計算
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
            var insPosY = groundY + insObj.transform.localScale.y / 2.0f;

            // 配置可能な座標を列挙
            for (var x = rangeMinX; x < rangeMaxX; x++)
            {
                for (var z = rangeMinZ; z <= rangeMaxZ; z++)
                {
                    var insPos = new Vector3(x, insPosY, z);
                    validPositions.Add(insPos);
                }
            }

            // 重複を避けた配置
            var carefulValidPos = validPositions.Where(pos =>
            {
                var minPos = CalcDiffPos(pos, -halfScaleX, -halfScaleZ);
                var maxPos = CalcDiffPos(pos, halfScaleX, halfScaleZ);
                return !placedGimmickList.Any(placed =>
                    minPos.x - PaddingThreshold < placed.InsMaxPos.x &&
                    maxPos.x + PaddingThreshold > placed.InsMinPos.x &&
                    minPos.z - PaddingThreshold < placed.InsMaxPos.z &&
                    maxPos.z + PaddingThreshold > placed.InsMinPos.z);
            }).ToList();

            if (!carefulValidPos.Any()) return;

            var careInsPos = carefulValidPos[UnityEngine.Random.Range(0, carefulValidPos.Count)];
            placedGimmickList.Add(new PlacedGimmickInfo
            {
                InsMinPos = CalcDiffPos(careInsPos, -halfScaleX, -halfScaleZ),
                InsMaxPos = CalcDiffPos(careInsPos, halfScaleX, halfScaleZ)
            });

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
                if (currentSize < size)
                {
                    mostBigRoom = i;
                    currentSize = size;
                }
            }
            return mostBigRoom;
        }
    }
}
