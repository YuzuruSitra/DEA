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
        private StageGenerator _stageGenerator;
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

        private int _roomCount;
        private int[,] _roomInfo;
        private float _groundY;

        private bool _onInitialized;
        private NavMeshHandler _navMeshHandler;
        private MetaAIHandler _metaAIHandler;
        [Header("特定のタイプの抽選確立")]
        [SerializeField] private int _typeProbability;
        
        public int GetObeliskRoom { get; private set; }
        public int GetRandomRoom => Random.Range(0, _roomCount);
        
        public void InitialGenerateGimmicks(StageGenerator stageGenerator, NavMeshHandler navMeshHandler)
        {
            InitializedRandomGimmick();
            _stageGenerator = stageGenerator;
            _roomCount = stageGenerator.RoomCount;
            _groundY = stageGenerator.GroundPosY;
            _roomInfo = stageGenerator.RoomInfo;
            _navMeshHandler = navMeshHandler;
            
            _inRoomGimmickPos = new List<PlacedGimmickInfo>[_roomCount];
            for (var i = 0; i < _roomCount; i++)
            {
                _inRoomGimmickPos[i] = new List<PlacedGimmickInfo>();
            }
            
            // ExitObeliskを生成
            GetObeliskRoom = CalcMostBigRoom(_roomCount, _roomInfo);
            InsGimmick(GetObeliskRoom, _gimmickInfo[(int)GimmickKind.ExitObelisk]._prefab);
            
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
                    var insType = DecideGimmickType(_metaAIHandler.CurrentPlayerType);
                    // 生成タイプの中から選定
                    var insNum = Random.Range(0, _insSeparateTypeGimmicks[insType].Count);
                    InsGimmick(i, _insSeparateTypeGimmicks[insType][insNum]._prefab);
                }
            }
        }

        private int DecideGimmickType(MetaAIHandler.PlayerType playerType)
        {
            // PlayerTypeが指定され、かつ指定確率以内ならそのタイプを返す
            if (playerType != MetaAIHandler.PlayerType.None && Random.Range(0, 101) <= _typeProbability)
            {
                return (int)playerType;
            }

            // PlayerType以外のギミックタイプをランダムに選択
            var playerTypeIndex = (int)playerType;
            var otherType = Random.Range(0, _metaAIHandler.PlayerTypeCount);
            // PlayerTypeと重ならないように調整
            if (otherType == playerTypeIndex)
            {
                otherType = (otherType + 1) % _metaAIHandler.PlayerTypeCount;
            }

            return otherType;
        }

        private void InitializedRandomGimmick()
        {
            _metaAIHandler = GameObject.FindWithTag("MetaAI").GetComponent<MetaAIHandler>();
            // ランダム配置ギミックの初期化
            var typeCount = _metaAIHandler.PlayerTypeCount;
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

        public GameObject InsGimmick(int roomNum, GameObject insObj)
        {
            var halfScaleX = insObj.transform.localScale.x / 2.0f;
            var halfScaleZ = insObj.transform.localScale.z / 2.0f;
            var insPosY = _groundY + insObj.transform.localScale.y / 2.0f;
            var gridStep = Mathf.CeilToInt(halfScaleX);

            for (var searchRoom = roomNum; searchRoom < _roomCount + roomNum; searchRoom++)
            {
                var currentRoom = searchRoom % _roomCount; 

                var paddingX = Mathf.CeilToInt(halfScaleX) + PaddingThreshold;
                var paddingZ = Mathf.CeilToInt(halfScaleZ) + PaddingThreshold;
                var rangeMinX = _roomInfo[currentRoom, (int)StageGenerator.RoomStatus.TopLeftX] + paddingX;
                var rangeMaxX = _roomInfo[currentRoom, (int)StageGenerator.RoomStatus.TopRightX] - (paddingX + 1);
                var rangeMinZ = _roomInfo[currentRoom, (int)StageGenerator.RoomStatus.BottomLeftZ] + paddingZ;
                var rangeMaxZ = _roomInfo[currentRoom, (int)StageGenerator.RoomStatus.TopLeftZ] - (paddingX + 1);

                var validPositions = new List<Vector3>();
                for (var x = rangeMinX; x < rangeMaxX; x += gridStep)
                {
                    for (var z = rangeMinZ; z <= rangeMaxZ; z += gridStep)
                    {
                        validPositions.Add(new Vector3(x, insPosY, z));
                    }
                }

                var carefulValidPos = validPositions.Where(pos =>
                {
                    var minPos = CalcDiffPos(pos, -halfScaleX, -halfScaleZ);
                    var maxPos = CalcDiffPos(pos, halfScaleX, halfScaleZ);
                    return !_inRoomGimmickPos[currentRoom].Any(placed =>
                        minPos.x - PaddingThreshold < placed.RightTopPos.x &&
                        maxPos.x + PaddingThreshold > placed.LeftBottomPos.x &&
                        minPos.z - PaddingThreshold < placed.RightTopPos.z &&
                        maxPos.z + PaddingThreshold > placed.LeftBottomPos.z);
                }).ToList();

                if (!carefulValidPos.Any()) continue;
                var careInsPos = carefulValidPos[Random.Range(0, carefulValidPos.Count)];
                var inRoomGimmickID = _inRoomGimmickPos[currentRoom].Count;
                _inRoomGimmickPos[currentRoom].Add(new PlacedGimmickInfo
                {
                    GimmickID = inRoomGimmickID,
                    LeftBottomPos = CalcDiffPos(careInsPos, -halfScaleX, -halfScaleZ),
                    RightTopPos = CalcDiffPos(careInsPos, halfScaleX, halfScaleZ)
                });

                var parent = _stageGenerator.NavMeshParents[currentRoom].transform;
                var insGimmick = Instantiate(insObj, careInsPos, insObj.transform.rotation, parent);
                var iGimmickID = insGimmick.GetComponent<IGimmickID>();
                _navMeshHandler.BakeTargetNavMesh(currentRoom);
                if (iGimmickID == null) return insGimmick;
                var info = iGimmickID.GimmickIdInfo;
                info.RoomID = currentRoom;
                info.InRoomGimmickID = inRoomGimmickID;
                iGimmickID.GimmickIdInfo = info;
                iGimmickID.Returned += RemoveRandomGimmickList;
                return insGimmick;
            }
            
            return null; // すべての部屋で配置できなかった場合
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
        
        // オブジェクトの破壊処理(別クラスの方がいい)
        public void OnDestroyList(List<GameObject> list)
        {
            if (list == null) return;
            foreach (var t in list)
            {
                if (t == null) continue;
                Destroy(t.gameObject);
            }
        }
    }
}
