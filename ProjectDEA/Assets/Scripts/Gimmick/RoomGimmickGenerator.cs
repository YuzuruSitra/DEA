using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Manager.Map;
using Manager.MetaAI;
using Random = UnityEngine.Random;

namespace Gimmick
{
    public class RoomGimmickGenerator : MonoBehaviour
    {
        private const int PaddingThreshold = 1;

        [Serializable]
        public struct GimmickInfo
        {
            public GimmickKind _kind;
            public GameObject _prefab;
            public bool _isRoomGenerate;
            public MetaAIHandler.PlayerType _priorityType;
        }
        [SerializeField] private GimmickInfo[] _gimmickInfo;
        public GimmickInfo[] GimmickInfos => _gimmickInfo;
        
        private List<GimmickInfo>[] _insSeparateTypeGimmicks;

        private struct PlacedGimmickInfo
        {
            public int GimmickID;
            public Vector3 LeftBottomPos;
            public Vector3 RightTopPos;
        }
        // ギミックは位置情報格納用
        private List<PlacedGimmickInfo>[] _inRoomGimmickPos;
        
        [SerializeField] private int _maxGimmickPerRoom;
        [SerializeField] private int _minGimmickPerRoom;
        [SerializeField] private Transform _mapParent;
        private const int InsKeyCount = 4;

        private int _roomCount;
        private int[,] _roomInfo;
        private float _groundY;

        private bool _onInitialized;
        
        public void InitialGenerateGimmicks(StageGenerator stageGenerator)
        {
            InitializedRandomGimmickList();
            
            _roomCount = stageGenerator.RoomCount;
            _groundY = stageGenerator.GroundPosY;
            _roomInfo = stageGenerator.RoomInfo;
            
            _inRoomGimmickPos = new List<PlacedGimmickInfo>[_roomCount];
            for (var i = 0; i < _roomCount; i++)
            {
                _inRoomGimmickPos[i] = new List<PlacedGimmickInfo>();
            }
            
            // ExitObeliskを生成
            var exitObeliskRoom = CalcMostBigRoom(_roomCount, _roomInfo);
            InsGimmick(exitObeliskRoom, _gimmickInfo[(int)GimmickKind.ExitObelisk]);

            // ObeliskKeyOutを生成
            GenerateObeliskKeys(_roomCount);
            
            _onInitialized = true;
        }

        public void RandomGenerateGimmicks(int playerRoomNum)
        {
            if (!_onInitialized) return;
            
            for (var i = 0; i < _roomCount; i++)
            {
                // プレイヤーと同じ部屋はスキップ
                if (i == playerRoomNum) continue;
                // 既存数が _minGimmickPerRoom を満たしている場合はスキップ
                if (_inRoomGimmickPos[i].Count >= _minGimmickPerRoom) continue;
                // 生成可能な最大数を計算
                var maxAllowed = _maxGimmickPerRoom - _inRoomGimmickPos[i].Count;
                // 生成数をランダムに決定（1以上、maxAllowed以下）
                var insCount = Random.Range(1, maxAllowed + 1);
                for (var n = 0; n < insCount; n++)
                {
                    // 生成タイプを決定
                    var insType = Random.Range(0, Enum.GetValues(typeof(MetaAIHandler.PlayerType)).Length);
                    // 生成タイプの中から選定
                    var insNum = Random.Range(0, _insSeparateTypeGimmicks[insType].Count);
                    InsGimmick(i, _insSeparateTypeGimmicks[insType][insNum]);
                }
            }
        }

        private void InitializedRandomGimmickList()
        {
            // ランダム配置ギミックの初期化
            var typeCount = Enum.GetValues(typeof(MetaAIHandler.PlayerType)).Length;
            _insSeparateTypeGimmicks = new List<GimmickInfo>[typeCount];
            // 各インデックスに List<GimmickInfo> を初期化
            for (var i = 0; i < typeCount; i++)
            {
                _insSeparateTypeGimmicks[i] = new List<GimmickInfo>();
            }
            
            foreach (var gimmick in _gimmickInfo)
            {
                if (!gimmick._isRoomGenerate) continue;
                _insSeparateTypeGimmicks[(int)gimmick._priorityType].Add(gimmick);
            }
        }

        private void RemoveRandomGimmickList(IGimmickID iGimmickID)
        {
            var id = iGimmickID.GimmickIdInfo;
            var targetList = _inRoomGimmickPos[id.RoomID];
            for (var i = 0; i < targetList.Count; i++)
            {
                var gimmick = targetList[i];
                if (gimmick.GimmickID != id.InRoomGimmickID) continue;
                _inRoomGimmickPos[id.RoomID].RemoveAt(i);
                break;
            }
            iGimmickID.Returned -= RemoveRandomGimmickList;
        }

        private void GenerateObeliskKeys(int roomCount)
        {
            var obeliskKeyRooms = Enumerable.Range(0, roomCount)
                                         .OrderBy(_ => Random.Range(0, roomCount))
                                         .Take(InsKeyCount)
                                         .ToArray();

            foreach (var roomIndex in obeliskKeyRooms)
            {
                InsGimmick(roomIndex, _gimmickInfo[(int)GimmickKind.ObeliskKeyOut]);
            }
        }

        private void InsGimmick(int roomNum, GimmickInfo gimmickInfo)
        {
            // 配置可能な範囲を計算
            var insObj = gimmickInfo._prefab;
            var halfScaleX = insObj.transform.localScale.x / 2.0f;
            var halfScaleZ = insObj.transform.localScale.z / 2.0f;
            var paddingX = Mathf.CeilToInt(halfScaleX) + PaddingThreshold;
            var paddingZ = Mathf.CeilToInt(halfScaleZ) + PaddingThreshold;
            var rangeMinX = _roomInfo[roomNum, (int)StageGenerator.RoomStatus.TopLeftX] + paddingX;
            var rangeMaxX = _roomInfo[roomNum, (int)StageGenerator.RoomStatus.TopRightX] - paddingX;
            var rangeMinZ = _roomInfo[roomNum, (int)StageGenerator.RoomStatus.BottomLeftZ] + paddingZ;
            var rangeMaxZ = _roomInfo[roomNum, (int)StageGenerator.RoomStatus.TopLeftZ] - paddingZ;

            var validPositions = new List<Vector3>();
            var insPosY = _groundY + insObj.transform.localScale.y / 2.0f;

            // 配置可能な座標を列挙（効率化：グリッドステップでスキップ）
            var gridStep = Mathf.CeilToInt(halfScaleX);
            for (var x = rangeMinX; x < rangeMaxX; x += gridStep)
            {
                for (var z = rangeMinZ; z <= rangeMaxZ; z += gridStep)
                {
                    var insPos = new Vector3(x, insPosY, z);
                    validPositions.Add(insPos);
                }
            }

            // 重複を避けた配置（効率化）
            var carefulValidPos = validPositions.Where(pos =>
            {
                var minPos = CalcDiffPos(pos, -halfScaleX, -halfScaleZ);
                var maxPos = CalcDiffPos(pos, halfScaleX, halfScaleZ);

                return !_inRoomGimmickPos[roomNum].Any(placed =>
                    minPos.x - PaddingThreshold < placed.RightTopPos.x &&
                    maxPos.x + PaddingThreshold > placed.LeftBottomPos.x &&
                    minPos.z - PaddingThreshold < placed.RightTopPos.z &&
                    maxPos.z + PaddingThreshold > placed.LeftBottomPos.z);
            }).ToList();

            if (!carefulValidPos.Any())
            {
                Debug.LogWarning($"No valid position found for room {roomNum}");
                return;
            }

            var careInsPos = carefulValidPos[Random.Range(0, carefulValidPos.Count)];
            var inRoomGimmickID = _inRoomGimmickPos[roomNum].Count;
            _inRoomGimmickPos[roomNum].Add(new PlacedGimmickInfo
            {
                GimmickID = inRoomGimmickID,
                LeftBottomPos = CalcDiffPos(careInsPos, -halfScaleX, -halfScaleZ),
                RightTopPos = CalcDiffPos(careInsPos, halfScaleX, halfScaleZ)
            });
            
            var insGimmick = Instantiate(insObj, careInsPos, insObj.transform.rotation, _mapParent).GetComponent<IGimmickID>();
            if (insGimmick == null) return;
            var info = insGimmick.GimmickIdInfo;
            info.RoomID = roomNum;
            info.InRoomGimmickID = inRoomGimmickID;
            insGimmick.GimmickIdInfo = info; 
            insGimmick.Returned += RemoveRandomGimmickList;
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
