using System;
using UnityEngine;

namespace Manager.Map
{
    public class StageGenerator : MonoBehaviour
    {
        [SerializeField] private Vector3 _defaultPosition;
        // Size of one side of the map.
        [SerializeField] private int _mapSize;
        // Block kind.
        private int[,] _mapKind;

        // Number of rooms.
        [SerializeField] private int _roomNum;
        // Minimum room size.
        [SerializeField] private int _roomMin = 4;
        [SerializeField] private int _heightCount;
        public int RoomCount { get; private set; }
        public float GroundPosY { get; private set; }

        private int _line; // 分割線
        public int[,] RoomInfo { get; private set; }
        [SerializeField] private Transform _mapParent;

        // 部屋ステータス
        public enum RoomStatus
        {
            X,// マップ座標
            Z,// マップ座標
            W,// 分割幅
            H,// 分割高さ

            Rx,// 部屋生成位置
            Rz,// 部屋生成位置
            Rw,// 部屋生成幅
            Rh,// 部屋生成高さ
            CenterX,       // 部屋中心X座標
            CenterZ,       // 部屋中心Y座標
            TopLeftX,
            TopLeftZ,
            TopRightX,
            TopRightZ,
            BottomLeftX,
            BottomLeftZ,
            BottomRightX,
            BottomRightZ
        }

        private enum ObjectType
        {
            Ground = 0,
            Wall = 1,
            Road = 2,
        }
    
        [SerializeField] private GameObject[] _mapObjects;

        // 壁から離す距離
        private const int OffsetWall = 2;
        private const int Offset = 1;
        
        public void MapGenerate()
        {
            GroundPosY = _defaultPosition.y + _mapObjects[(int)ObjectType.Ground].transform.localScale.y / 2.0f;

            RoomInfo = new int[_roomNum, Enum.GetNames(typeof(RoomStatus)).Length];
            
            _mapKind = new int[_mapSize, _mapSize];
            
            // フロアの初期化
            for (var nowW = 0; nowW < _mapSize; nowW++)
            {
                for (var nowH = 0; nowH < _mapSize; nowH++)
                {
                    _mapKind[nowW, nowH] = (int)ObjectType.Road;
                }
            }

            RoomInfo[RoomCount, (int)RoomStatus.X] = 0;
            RoomInfo[RoomCount, (int)RoomStatus.Z] = 0;
            RoomInfo[RoomCount, (int)RoomStatus.W] = _mapSize;
            RoomInfo[RoomCount, (int)RoomStatus.H] = _mapSize;

            RoomCount++;

            // 部屋分割
            for (var splitNum = 0; splitNum < _roomNum - 1; splitNum++)
            {
                var parentNum = 0;
                var max = 0;

                // 最大の部屋番号を調べる
                for (var maxCheck = 0; maxCheck < _roomNum; maxCheck++)
                {
                    if (max >= RoomInfo[maxCheck, (int)RoomStatus.W] * RoomInfo[maxCheck, (int)RoomStatus.H])
                        continue;
                    max = RoomInfo[maxCheck, (int)RoomStatus.W] * RoomInfo[maxCheck, (int)RoomStatus.H];
                    
                    parentNum = maxCheck;
                }

                // 取得した部屋をさらに分割
                if (SplitPoint(RoomInfo[parentNum, (int)RoomStatus.W], RoomInfo[parentNum, (int)RoomStatus.H]))
                {
                    RoomInfo[RoomCount, (int)RoomStatus.X] = RoomInfo[parentNum, (int)RoomStatus.X];
                    RoomInfo[RoomCount, (int)RoomStatus.Z] = RoomInfo[parentNum, (int)RoomStatus.Z];
                    RoomInfo[RoomCount, (int)RoomStatus.W] = RoomInfo[parentNum, (int)RoomStatus.W] - _line;
                    RoomInfo[RoomCount, (int)RoomStatus.H] = RoomInfo[parentNum, (int)RoomStatus.H];

                    // 親の部屋を整形する
                    RoomInfo[parentNum, (int)RoomStatus.X] += RoomInfo[RoomCount, (int)RoomStatus.W];
                    RoomInfo[parentNum, (int)RoomStatus.W] -= RoomInfo[RoomCount, (int)RoomStatus.W];
                }
                else
                {
                    RoomInfo[RoomCount, (int)RoomStatus.X] = RoomInfo[parentNum, (int)RoomStatus.X];
                    RoomInfo[RoomCount, (int)RoomStatus.Z] = RoomInfo[parentNum, (int)RoomStatus.Z];
                    RoomInfo[RoomCount, (int)RoomStatus.W] = RoomInfo[parentNum, (int)RoomStatus.W];
                    RoomInfo[RoomCount, (int)RoomStatus.H] = RoomInfo[parentNum, (int)RoomStatus.H] - _line;

                    // 親の部屋を整形する
                    RoomInfo[parentNum, (int)RoomStatus.Z] += RoomInfo[RoomCount, (int)RoomStatus.H];
                    RoomInfo[parentNum, (int)RoomStatus.H] -= RoomInfo[RoomCount, (int)RoomStatus.H];
                }
                RoomCount++;
            }

            // 分割した部屋の中にランダムな部屋を生成
            for (var i = 0; i < _roomNum; i++)
            {
                // 生成座標の設定
                RoomInfo[i, (int)RoomStatus.Rx] = UnityEngine.Random.Range(RoomInfo[i, (int)RoomStatus.X] + OffsetWall, (RoomInfo[i, (int)RoomStatus.X] + RoomInfo[i, (int)RoomStatus.W]) - (_roomMin + OffsetWall));
                RoomInfo[i, (int)RoomStatus.Rz] = UnityEngine.Random.Range(RoomInfo[i, (int)RoomStatus.Z] + OffsetWall, (RoomInfo[i, (int)RoomStatus.Z] + RoomInfo[i, (int)RoomStatus.H]) - (_roomMin + OffsetWall));
                
                RoomInfo[i, (int)RoomStatus.Rw] = UnityEngine.Random.Range(_roomMin, RoomInfo[i, (int)RoomStatus.W] - (RoomInfo[i, (int)RoomStatus.Rx] - RoomInfo[i, (int)RoomStatus.X]) - Offset);
                RoomInfo[i, (int)RoomStatus.Rh] = UnityEngine.Random.Range(_roomMin, RoomInfo[i, (int)RoomStatus.H] - (RoomInfo[i, (int)RoomStatus.Rz] - RoomInfo[i, (int)RoomStatus.Z]) - Offset);
                
                CalculateRoomCoordinates(i);
            }

            // マップ上書き
            for (var count = 0; count < _roomNum; count++)
            {
                for (var h = 0; h < RoomInfo[count, (int)RoomStatus.H]; h++)
                {
                    for (var w = 0; w < RoomInfo[count, (int)RoomStatus.W]; w++)
                    {
                        _mapKind[w + RoomInfo[count, (int)RoomStatus.X], h + RoomInfo[count, (int)RoomStatus.Z]] = (int)ObjectType.Wall;
                    }

                }
                
                for (var h = 0; h < RoomInfo[count, (int)RoomStatus.Rh]; h++)
                {
                    for (var w = 0; w < RoomInfo[count, (int)RoomStatus.Rw]; w++)
                    {
                        _mapKind[w + RoomInfo[count, (int)RoomStatus.Rx], h + RoomInfo[count, (int)RoomStatus.Rz]] = (int)ObjectType.Ground;
                    }
                }
            }

            // 道生成
            var splitLength = new int[4];

            // 部屋から一番近い境界線を調べる
            for (var nowRoom = 0; nowRoom < _roomNum; nowRoom++)
            {
                // 左の壁から
                splitLength[0] = RoomInfo[nowRoom, (int)RoomStatus.X] > 0 ?
                    RoomInfo[nowRoom, (int)RoomStatus.Rx] - RoomInfo[nowRoom, (int)RoomStatus.X] : int.MaxValue;
                // 右の壁から
                splitLength[1] = (RoomInfo[nowRoom, (int)RoomStatus.X] + RoomInfo[nowRoom, (int)RoomStatus.W]) < _mapSize ?
                    (RoomInfo[nowRoom, (int)RoomStatus.X] + RoomInfo[nowRoom, (int)RoomStatus.W]) - (RoomInfo[nowRoom, (int)RoomStatus.Rx] + RoomInfo[nowRoom, (int)RoomStatus.Rw]) : int.MaxValue;

                // 下の壁から
                splitLength[2] = RoomInfo[nowRoom, (int)RoomStatus.Z] > 0 ?
                    RoomInfo[nowRoom, (int)RoomStatus.Rz] - RoomInfo[nowRoom, (int)RoomStatus.Z] : int.MaxValue;
                // 上の壁から
                splitLength[3] = (RoomInfo[nowRoom, (int)RoomStatus.Z] + RoomInfo[nowRoom, (int)RoomStatus.H]) < _mapSize ?
                    (RoomInfo[nowRoom, (int)RoomStatus.Z] + RoomInfo[nowRoom, (int)RoomStatus.H]) - (RoomInfo[nowRoom, (int)RoomStatus.Rz] + RoomInfo[nowRoom, (int)RoomStatus.Rh]) : int.MaxValue;
                
                for (var j = 0; j < splitLength.Length; j++)
                {
                    if (splitLength[j] == int.MaxValue) continue;
                    // 上下左右判定
                    int roodPoint;
                    if (j < 2)
                    {
                        // 道を引く場所を決める
                        roodPoint = UnityEngine.Random.Range(RoomInfo[nowRoom, (int)RoomStatus.Rz] + Offset, RoomInfo[nowRoom, (int)RoomStatus.Rz] + RoomInfo[nowRoom, (int)RoomStatus.Rh] - Offset);

                        // マップに書き込む
                        for (var w = 1; w <= splitLength[j]; w++)
                        {
                            // 左右判定
                            if (j == 0)
                            {
                                // 左
                                _mapKind[(-w) + RoomInfo[nowRoom, (int)RoomStatus.Rx], roodPoint] = (int)ObjectType.Road;
                            }
                            else
                            {
                                // 右
                                _mapKind[w + RoomInfo[nowRoom, (int)RoomStatus.Rx] + RoomInfo[nowRoom, (int)RoomStatus.Rw] - Offset, roodPoint] = (int)ObjectType.Road;
                                
                                if (w == splitLength[j])
                                {
                                    // 一つ多く作る
                                    _mapKind[w + Offset + RoomInfo[nowRoom, (int)RoomStatus.Rx] + RoomInfo[nowRoom, (int)RoomStatus.Rw] - Offset, roodPoint] = (int)ObjectType.Road;
                                }
                            }
                        }
                    }
                    else
                    {
                        // 道を引く場所を決める
                        roodPoint = UnityEngine.Random.Range(RoomInfo[nowRoom, (int)RoomStatus.Rx] + Offset, RoomInfo[nowRoom, (int)RoomStatus.Rx] + RoomInfo[nowRoom, (int)RoomStatus.Rw] - Offset);

                        // マップに書き込む
                        for (var h = 1; h <= splitLength[j]; h++)
                        {
                            // 上下判定
                            if (j == 2)
                            {
                                _mapKind[roodPoint, (-h) + RoomInfo[nowRoom, (int)RoomStatus.Rz]] = (int)ObjectType.Road;
                            }
                            else
                            {
                                _mapKind[roodPoint, h + RoomInfo[nowRoom, (int)RoomStatus.Rz] + RoomInfo[nowRoom, (int)RoomStatus.Rh] - Offset] = (int)ObjectType.Road;
                                
                                if (h == splitLength[j])
                                {
                                    _mapKind[roodPoint, h + Offset + RoomInfo[nowRoom, (int)RoomStatus.Rz] + RoomInfo[nowRoom, (int)RoomStatus.Rh] - Offset] = (int)ObjectType.Road;
                                }
                            }
                        }
                    }
                }
            }
            
            for (var nowRoom = 0; nowRoom < _roomNum; nowRoom++)
            {
                var roadVec1 = 0;
                var roadVec2 = 0;
                // 道を繋げる
                for (var roodScan = 0; roodScan < RoomInfo[nowRoom, (int)RoomStatus.W]; roodScan++)
                {
                    // 道を検索
                    if (_mapKind[roodScan + RoomInfo[nowRoom, (int)RoomStatus.X],
                            RoomInfo[nowRoom, (int)RoomStatus.Z]] != (int)ObjectType.Road) continue;
                    if (roadVec1 == 0)
                    {
                        roadVec1 = roodScan + RoomInfo[nowRoom, (int)RoomStatus.X];
                    }
                    else
                    {
                        roadVec2 = roodScan + RoomInfo[nowRoom, (int)RoomStatus.X];
                    }
                }
                // 道を引く
                for (var roadSet = roadVec1; roadSet < roadVec2; roadSet++)
                {
                    _mapKind[roadSet, RoomInfo[nowRoom, (int)RoomStatus.Z]] = (int)ObjectType.Road;
                }

                roadVec1 = 0;
                roadVec2 = 0;

                for (var roadScan = 0; roadScan < RoomInfo[nowRoom, (int)RoomStatus.H]; roadScan++)
                {
                    // 道を検索
                    if (_mapKind[RoomInfo[nowRoom, (int)RoomStatus.X],
                            roadScan + RoomInfo[nowRoom, (int)RoomStatus.Z]] != (int)ObjectType.Road) continue;
                    if (roadVec1 == 0)
                    {
                        roadVec1 = roadScan + RoomInfo[nowRoom, (int)RoomStatus.Z];
                    }
                    else
                    {
                        roadVec2 = roadScan + RoomInfo[nowRoom, (int)RoomStatus.Z];
                    }
                }
                // 道を引く
                for (var roadSet = roadVec1; roadSet < roadVec2; roadSet++)
                {
                    // 境界線を上書き
                    _mapKind[RoomInfo[nowRoom, (int)RoomStatus.X], roadSet] = (int)ObjectType.Road;
                }
            }
            
            // オブジェクトを生成する
            for (var nowH = 0; nowH < _mapSize; nowH++)
            {
                for (var nowW = 0; nowW < _mapSize; nowW++)
                {
                    // 壁を生成
                    if (_mapKind[nowW, nowH] == (int)ObjectType.Wall)
                    {
                        var paddingY = 0.0f;
                        var addValue = _mapObjects[(int)ObjectType.Wall].transform.localScale.y;
                        for (var i = 0; i < _heightCount; i++)
                        {
                            Instantiate(
                                _mapObjects[_mapKind[nowW, nowH]],
                                new Vector3(
                                    _defaultPosition.x + nowW * _mapObjects[_mapKind[nowW, nowH]].transform.localScale.x,
                                    _defaultPosition.y + paddingY,
                                    _defaultPosition.z + nowH * _mapObjects[_mapKind[nowW, nowH]].transform.localScale.z),
                                Quaternion.identity, _mapParent);
                            paddingY += addValue;
                        }
                    }

                    // 部屋を生成
                    if (_mapKind[nowW, nowH] == (int)ObjectType.Ground)
                    {
                        Instantiate(
                            _mapObjects[_mapKind[nowW, nowH]],
                            new Vector3(
                                _defaultPosition.x + nowW * _mapObjects[_mapKind[nowW, nowH]].transform.localScale.x,
                                _defaultPosition.y,
                                _defaultPosition.z + nowH * _mapObjects[_mapKind[nowW, nowH]].transform.localScale.z),
                            Quaternion.identity, _mapParent);
                    }

                    // 通路の生成
                    if (_mapKind[nowW, nowH] == (int)ObjectType.Road)
                    {
                        Instantiate(
                            _mapObjects[_mapKind[nowW, nowH]],
                            new Vector3(
                                _defaultPosition.x + nowW * _mapObjects[_mapKind[nowW, nowH]].transform.localScale.x,
                                _defaultPosition.y,
                                _defaultPosition.z + nowH * _mapObjects[_mapKind[nowW, nowH]].transform.localScale.z),
                            Quaternion.identity, _mapParent);
                    }

                }
            }
        }
        
        // 部屋情報の計算処理
        private void CalculateRoomCoordinates(int roomIndex)
        {
            var roomX = RoomInfo[roomIndex, (int)RoomStatus.Rx];
            var roomZ = RoomInfo[roomIndex, (int)RoomStatus.Rz];
            var roomW = RoomInfo[roomIndex, (int)RoomStatus.Rw];
            var roomH = RoomInfo[roomIndex, (int)RoomStatus.Rh];
            
            var floorScaleX = (int)_mapObjects[(int)ObjectType.Ground].transform.localScale.x;
            var floorScaleZ = (int)_mapObjects[(int)ObjectType.Ground].transform.localScale.z;
            var centerX = (int) (roomX + roomW / 2f);
            var centerZ = (int) (roomZ + roomH / 2f);
            RoomInfo[roomIndex, (int)RoomStatus.CenterX] = (int)_defaultPosition.x + centerX * floorScaleX;
            RoomInfo[roomIndex, (int)RoomStatus.CenterZ] = (int)_defaultPosition.z + centerZ * floorScaleZ;
        
            RoomInfo[roomIndex, (int)RoomStatus.TopLeftX] = (int)_defaultPosition.x + roomX * floorScaleX  - floorScaleX;
            RoomInfo[roomIndex, (int)RoomStatus.TopLeftZ] = (int)_defaultPosition.z + (roomZ + roomH) * floorScaleZ;
        
            RoomInfo[roomIndex, (int)RoomStatus.TopRightX] = (int)_defaultPosition.x + (roomX + roomW) * floorScaleX;
            RoomInfo[roomIndex, (int)RoomStatus.TopRightZ] = (int)_defaultPosition.z + (roomZ + roomH) * floorScaleZ;
        
            RoomInfo[roomIndex, (int)RoomStatus.BottomLeftX] = (int)_defaultPosition.x + roomX * floorScaleX  - floorScaleX;
            RoomInfo[roomIndex, (int)RoomStatus.BottomLeftZ] = (int)_defaultPosition.z + roomZ * floorScaleZ - floorScaleZ;
        
            RoomInfo[roomIndex, (int)RoomStatus.BottomRightX] = (int)_defaultPosition.x + (roomX + roomW) * floorScaleX;
            RoomInfo[roomIndex, (int)RoomStatus.BottomRightZ] = (int)_defaultPosition.z + roomZ * floorScaleZ - floorScaleZ;
        }
        
        private bool SplitPoint(int x, int y)
        {
            if (x > y)
            {
                _line = UnityEngine.Random.Range(_roomMin + (OffsetWall * 2), x - (OffsetWall * 2 + _roomMin));// 縦割�?
                return true;
            }
            _line = UnityEngine.Random.Range(_roomMin + (OffsetWall * 2), y - (OffsetWall * 2 + _roomMin));// 横割�?
            return false;
        }
    }
}