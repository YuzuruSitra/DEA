using UnityEngine;

namespace System.Map
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
        public int RoomCount { get; private set; }

        private int _line; // 分割点
        public int[,] RoomInfo { get; private set; }
        [SerializeField] private Transform _mapParent;

        public enum RoomStatus                     // 部屋の配列ステータス
        {
            X,// マップ座標ｘ
            Z,// マップ座標ｙ
            W,// 分割した幅
            H,// 分割した高さ

            Rx,// 部屋の生成位置
            Rz,// 部屋の生成位置
            Rw,// 部屋の幅
            Rh,// 部屋の高さ
            CenterX,       // 部屋の中心X座標
            CenterZ,       // 部屋の中心Y座標
            TopLeftX,      // 左上X座標
            TopLeftZ,      // 左上Y座標
            TopRightX,     // 右上X座標
            TopRightZ,     // 右上Y座標
            BottomLeftX,   // 左下X座標
            BottomLeftZ,   // 左下Y座標
            BottomRightX,  // 右下X座標
            BottomRightZ   // 右下Y座標
        }

        private enum ObjectType
        {
            Ground = 0,
            Wall = 1,
            Road = 2,
        }
    
        [SerializeField] private GameObject[] _mapObjects;               // マップ生成用のオブジェクト配列

        private const int OffsetWall = 2;   // 壁から離す距離
        private const int Offset = 1;       // 調整用
        
        public void MapGenerate()
        {
            // 部屋（StartX、StartY、幅、高さ）
            RoomInfo = new int[_roomNum, Enum.GetNames(typeof(RoomStatus)).Length];

            // フロア設定
            _mapKind = new int[_mapSize, _mapSize];


            // フロアの初期化
            for (var nowW = 0; nowW < _mapSize; nowW++)
            {
                for (var nowH = 0; nowH < _mapSize; nowH++)
                {
                    _mapKind[nowW, nowH] = 2;
                }
            }

            // フロアを入れる
            RoomInfo[RoomCount, (int)RoomStatus.X] = 0;
            RoomInfo[RoomCount, (int)RoomStatus.Z] = 0;
            RoomInfo[RoomCount, (int)RoomStatus.W] = _mapSize;
            RoomInfo[RoomCount, (int)RoomStatus.H] = _mapSize;

            // カウント追加
            RoomCount++;

            // 部屋の数だけ分割する
            for (var splitNum = 0; splitNum < _roomNum - 1; splitNum++)
            {
                // 変数初期化
                var parentNum = 0;
                var max = 0;

                // 最大の部屋番号を調べる
                for (var maxCheck = 0; maxCheck < _roomNum; maxCheck++)
                {
                    // 面積比較
                    if (max >= RoomInfo[maxCheck, (int)RoomStatus.W] * RoomInfo[maxCheck, (int)RoomStatus.H])
                        continue;
                    // 最大面積上書き
                    max = RoomInfo[maxCheck, (int)RoomStatus.W] * RoomInfo[maxCheck, (int)RoomStatus.H];

                    // 親の部屋番号セット
                    parentNum = maxCheck;
                }

                // 取得した部屋をさらに割る
                if (SplitPoint(RoomInfo[parentNum, (int)RoomStatus.W], RoomInfo[parentNum, (int)RoomStatus.H]))
                {
                    // 取得
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
                    // 取得
                    RoomInfo[RoomCount, (int)RoomStatus.X] = RoomInfo[parentNum, (int)RoomStatus.X];
                    RoomInfo[RoomCount, (int)RoomStatus.Z] = RoomInfo[parentNum, (int)RoomStatus.Z];
                    RoomInfo[RoomCount, (int)RoomStatus.W] = RoomInfo[parentNum, (int)RoomStatus.W];
                    RoomInfo[RoomCount, (int)RoomStatus.H] = RoomInfo[parentNum, (int)RoomStatus.H] - _line;

                    // 親の部屋を整形する
                    RoomInfo[parentNum, (int)RoomStatus.Z] += RoomInfo[RoomCount, (int)RoomStatus.H];
                    RoomInfo[parentNum, (int)RoomStatus.H] -= RoomInfo[RoomCount, (int)RoomStatus.H];
                }
                // カウントを加算
                RoomCount++;
            }

            // 分割した中にランダムな大きさの部屋を生成
            for (var i = 0; i < _roomNum; i++)
            {
                // 生成座標の設定
                RoomInfo[i, (int)RoomStatus.Rx] = UnityEngine.Random.Range(RoomInfo[i, (int)RoomStatus.X] + OffsetWall, (RoomInfo[i, (int)RoomStatus.X] + RoomInfo[i, (int)RoomStatus.W]) - (_roomMin + OffsetWall));
                RoomInfo[i, (int)RoomStatus.Rz] = UnityEngine.Random.Range(RoomInfo[i, (int)RoomStatus.Z] + OffsetWall, (RoomInfo[i, (int)RoomStatus.Z] + RoomInfo[i, (int)RoomStatus.H]) - (_roomMin + OffsetWall));

                // 部屋の大きさを設定
                RoomInfo[i, (int)RoomStatus.Rw] = UnityEngine.Random.Range(_roomMin, RoomInfo[i, (int)RoomStatus.W] - (RoomInfo[i, (int)RoomStatus.Rx] - RoomInfo[i, (int)RoomStatus.X]) - Offset);
                RoomInfo[i, (int)RoomStatus.Rh] = UnityEngine.Random.Range(_roomMin, RoomInfo[i, (int)RoomStatus.H] - (RoomInfo[i, (int)RoomStatus.Rz] - RoomInfo[i, (int)RoomStatus.Z]) - Offset);
                
                // 部屋の中心座標と4隅の座標を計算
                CalculateRoomCoordinates(i);
            }

            // マップ上書き
            for (var count = 0; count < _roomNum; count++)
            {
                // 取得した部屋の確認
                for (var h = 0; h < RoomInfo[count, (int)RoomStatus.H]; h++)
                {
                    for (var w = 0; w < RoomInfo[count, (int)RoomStatus.W]; w++)
                    {
                        // 部屋チェックポイント
                        _mapKind[w + RoomInfo[count, (int)RoomStatus.X], h + RoomInfo[count, (int)RoomStatus.Z]] = (int)ObjectType.Wall;
                    }

                }

                // 生成した部屋
                for (var h = 0; h < RoomInfo[count, (int)RoomStatus.Rh]; h++)
                {
                    for (var w = 0; w < RoomInfo[count, (int)RoomStatus.Rw]; w++)
                    {
                        _mapKind[w + RoomInfo[count, (int)RoomStatus.Rx], h + RoomInfo[count, (int)RoomStatus.Rz]] = (int)ObjectType.Ground;
                    }
                }
            }

            // 道の生成
            var splitLength = new int[4];

            // 部屋から一番近い境界線を調べる(十字に調べる)
            for (var nowRoom = 0; nowRoom < _roomNum; nowRoom++)
            {
                // 左の壁からの距離
                splitLength[0] = RoomInfo[nowRoom, (int)RoomStatus.X] > 0 ?
                    RoomInfo[nowRoom, (int)RoomStatus.Rx] - RoomInfo[nowRoom, (int)RoomStatus.X] : int.MaxValue;
                // 右の壁からの距離
                splitLength[1] = (RoomInfo[nowRoom, (int)RoomStatus.X] + RoomInfo[nowRoom, (int)RoomStatus.W]) < _mapSize ?
                    (RoomInfo[nowRoom, (int)RoomStatus.X] + RoomInfo[nowRoom, (int)RoomStatus.W]) - (RoomInfo[nowRoom, (int)RoomStatus.Rx] + RoomInfo[nowRoom, (int)RoomStatus.Rw]) : int.MaxValue;

                // 下の壁からの距離
                splitLength[2] = RoomInfo[nowRoom, (int)RoomStatus.Z] > 0 ?
                    RoomInfo[nowRoom, (int)RoomStatus.Rz] - RoomInfo[nowRoom, (int)RoomStatus.Z] : int.MaxValue;
                // 上の壁からの距離
                splitLength[3] = (RoomInfo[nowRoom, (int)RoomStatus.Z] + RoomInfo[nowRoom, (int)RoomStatus.H]) < _mapSize ?
                    (RoomInfo[nowRoom, (int)RoomStatus.Z] + RoomInfo[nowRoom, (int)RoomStatus.H]) - (RoomInfo[nowRoom, (int)RoomStatus.Rz] + RoomInfo[nowRoom, (int)RoomStatus.Rh]) : int.MaxValue;

                // マックスでない物のみ先へ
                for (var j = 0; j < splitLength.Length; j++)
                {
                    if (splitLength[j] == int.MaxValue) continue;
                    // 上下左右判定
                    int roodPoint;// 道を引く場所
                    if (j < 2)
                    {
                        // 道を引く場所を決定
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

                                // 最後
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
                        // 道を引く場所を決定
                        roodPoint = UnityEngine.Random.Range(RoomInfo[nowRoom, (int)RoomStatus.Rx] + Offset, RoomInfo[nowRoom, (int)RoomStatus.Rx] + RoomInfo[nowRoom, (int)RoomStatus.Rw] - Offset);

                        // マップに書き込む
                        for (var h = 1; h <= splitLength[j]; h++)
                        {
                            // 上下判定
                            if (j == 2)
                            {
                                // 下
                                _mapKind[roodPoint, (-h) + RoomInfo[nowRoom, (int)RoomStatus.Rz]] = (int)ObjectType.Road;
                            }
                            else
                            {
                                // 上
                                _mapKind[roodPoint, h + RoomInfo[nowRoom, (int)RoomStatus.Rz] + RoomInfo[nowRoom, (int)RoomStatus.Rh] - Offset] = (int)ObjectType.Road;

                                // 最後
                                if (h == splitLength[j])
                                {
                                    // 一つ多く作る
                                    _mapKind[roodPoint, h + Offset + RoomInfo[nowRoom, (int)RoomStatus.Rz] + RoomInfo[nowRoom, (int)RoomStatus.Rh] - Offset] = (int)ObjectType.Road;
                                }
                            }
                        }
                    }
                }
            }

            // 道の接続
            for (var nowRoom = 0; nowRoom < _roomNum; nowRoom++)
            {
                var roadVec1 = 0;// 道の始点
                var roadVec2 = 0;// 道の終点
                // 道を繋げる
                for (var roodScan = 0; roodScan < RoomInfo[nowRoom, (int)RoomStatus.W]; roodScan++)
                {
                    // 道を検索
                    if (_mapKind[roodScan + RoomInfo[nowRoom, (int)RoomStatus.X],
                            RoomInfo[nowRoom, (int)RoomStatus.Z]] != (int)ObjectType.Road) continue;
                    // 道の座標セット
                    if (roadVec1 == 0)
                    {
                        // 始点セット
                        roadVec1 = roodScan + RoomInfo[nowRoom, (int)RoomStatus.X];
                    }
                    else
                    {
                        // 終点セット
                        roadVec2 = roodScan + RoomInfo[nowRoom, (int)RoomStatus.X];
                    }
                }
                // 道を引く
                for (var roadSet = roadVec1; roadSet < roadVec2; roadSet++)
                {
                    // 境界線を上書き
                    _mapKind[roadSet, RoomInfo[nowRoom, (int)RoomStatus.Z]] = (int)ObjectType.Road;
                }

                roadVec1 = 0;
                roadVec2 = 0;

                for (var roadScan = 0; roadScan < RoomInfo[nowRoom, (int)RoomStatus.H]; roadScan++)
                {
                    // 道を検索
                    if (_mapKind[RoomInfo[nowRoom, (int)RoomStatus.X],
                            roadScan + RoomInfo[nowRoom, (int)RoomStatus.Z]] != (int)ObjectType.Road) continue;
                    // 道の座標セット
                    if (roadVec1 == 0)
                    {
                        // 始点セット
                        roadVec1 = roadScan + RoomInfo[nowRoom, (int)RoomStatus.Z];
                    }
                    else
                    {
                        // 終点セット
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
                    // 壁の生成
                    if (_mapKind[nowW, nowH] == (int)ObjectType.Wall)
                    {
                        Instantiate(
                            _mapObjects[_mapKind[nowW, nowH]],
                            new Vector3(
                                _defaultPosition.x + nowW * _mapObjects[_mapKind[nowW, nowH]].transform.localScale.x,
                                _defaultPosition.y + (_mapObjects[(int)ObjectType.Wall].transform.localScale.y - _mapObjects[(int)ObjectType.Ground].transform.localScale.y) * 0.5f,
                                _defaultPosition.z + nowH * _mapObjects[_mapKind[nowW, nowH]].transform.localScale.z),
                            Quaternion.identity, _mapParent);
                    }

                    // 部屋の生成
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
        
        // 部屋の中心座標と4隅の座標を計算するメソッド
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

        // 分割点のセット(int x, int y)、大きい方を分割する
        private bool SplitPoint(int x, int y)
        {
            // 分割位置の決定
            if (x > y)
            {
                _line = UnityEngine.Random.Range(_roomMin + (OffsetWall * 2), x - (OffsetWall * 2 + _roomMin));// 縦割り
                return true;
            }
            else
            {
                _line = UnityEngine.Random.Range(_roomMin + (OffsetWall * 2), y - (OffsetWall * 2 + _roomMin));// 横割り
                return false;
            }
        }
    }
}