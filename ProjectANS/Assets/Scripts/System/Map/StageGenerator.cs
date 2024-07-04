using UnityEngine;

namespace System.Map
{
    public class StageGenerator : MonoBehaviour
    {
        [SerializeField] Vector3 _defaultPosition;
        // Size of one side of the map.
        [SerializeField] private int _mapSize;
        // Block kind.
        private int[,] _mapKind;

        // Number of rooms.
        [SerializeField] private int _roomNum;
        // Minimum room size.
        [SerializeField] private int _roomMin = 4;
        private int _roomCount;                      // 部屋カウント
        private int _line; // 分割点
        private int[,] _roomInfo;

        private enum RoomStatus                     // 部屋の配列ステータス
        {
            X,// マップ座標ｘ
            Y,// マップ座標ｙ
            W,// 分割した幅
            H,// 分割した高さ

            Rx,// 部屋の生成位置
            Ry,// 部屋の生成位置
            RW,// 部屋の幅
            Rh,// 部屋の高さ
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
            _roomInfo = new int[_roomNum, Enum.GetNames(typeof(RoomStatus)).Length];

            // フロア設定
            _mapKind = new int[_mapSize, _mapSize];


            // フロアの初期化
            for (var nowW = 0; nowW < _mapSize; nowW++)
            {
                for (var nowH = 0; nowH < _mapSize; nowH++)
                {
                    // 壁を貼る
                    _mapKind[nowW, nowH] = 2;
                }
            }

            // フロアを入れる
            _roomInfo[_roomCount, (int)RoomStatus.X] = 0;
            _roomInfo[_roomCount, (int)RoomStatus.Y] = 0;
            _roomInfo[_roomCount, (int)RoomStatus.W] = _mapSize;
            _roomInfo[_roomCount, (int)RoomStatus.H] = _mapSize;

            // カウント追加
            _roomCount++;

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
                    if (max >= _roomInfo[maxCheck, (int)RoomStatus.W] * _roomInfo[maxCheck, (int)RoomStatus.H])
                        continue;
                    // 最大面積上書き
                    max = _roomInfo[maxCheck, (int)RoomStatus.W] * _roomInfo[maxCheck, (int)RoomStatus.H];

                    // 親の部屋番号セット
                    parentNum = maxCheck;
                }

                // 取得した部屋をさらに割る
                if (SplitPoint(_roomInfo[parentNum, (int)RoomStatus.W], _roomInfo[parentNum, (int)RoomStatus.H]))
                {
                    // 取得
                    _roomInfo[_roomCount, (int)RoomStatus.X] = _roomInfo[parentNum, (int)RoomStatus.X];
                    _roomInfo[_roomCount, (int)RoomStatus.Y] = _roomInfo[parentNum, (int)RoomStatus.Y];
                    _roomInfo[_roomCount, (int)RoomStatus.W] = _roomInfo[parentNum, (int)RoomStatus.W] - _line;
                    _roomInfo[_roomCount, (int)RoomStatus.H] = _roomInfo[parentNum, (int)RoomStatus.H];

                    // 親の部屋を整形する
                    _roomInfo[parentNum, (int)RoomStatus.X] += _roomInfo[_roomCount, (int)RoomStatus.W];
                    _roomInfo[parentNum, (int)RoomStatus.W] -= _roomInfo[_roomCount, (int)RoomStatus.W];
                }
                else
                {
                    // 取得
                    _roomInfo[_roomCount, (int)RoomStatus.X] = _roomInfo[parentNum, (int)RoomStatus.X];
                    _roomInfo[_roomCount, (int)RoomStatus.Y] = _roomInfo[parentNum, (int)RoomStatus.Y];
                    _roomInfo[_roomCount, (int)RoomStatus.W] = _roomInfo[parentNum, (int)RoomStatus.W];
                    _roomInfo[_roomCount, (int)RoomStatus.H] = _roomInfo[parentNum, (int)RoomStatus.H] - _line;

                    // 親の部屋を整形する
                    _roomInfo[parentNum, (int)RoomStatus.Y] += _roomInfo[_roomCount, (int)RoomStatus.H];
                    _roomInfo[parentNum, (int)RoomStatus.H] -= _roomInfo[_roomCount, (int)RoomStatus.H];
                }
                // カウントを加算
                _roomCount++;
            }

            // 分割した中にランダムな大きさの部屋を生成
            for (var i = 0; i < _roomNum; i++)
            {
                // 生成座標の設定
                _roomInfo[i, (int)RoomStatus.Rx] = UnityEngine.Random.Range(_roomInfo[i, (int)RoomStatus.X] + OffsetWall, (_roomInfo[i, (int)RoomStatus.X] + _roomInfo[i, (int)RoomStatus.W]) - (_roomMin + OffsetWall));
                _roomInfo[i, (int)RoomStatus.Ry] = UnityEngine.Random.Range(_roomInfo[i, (int)RoomStatus.Y] + OffsetWall, (_roomInfo[i, (int)RoomStatus.Y] + _roomInfo[i, (int)RoomStatus.H]) - (_roomMin + OffsetWall));

                // 部屋の大きさを設定
                _roomInfo[i, (int)RoomStatus.RW] = UnityEngine.Random.Range(_roomMin, _roomInfo[i, (int)RoomStatus.W] - (_roomInfo[i, (int)RoomStatus.Rx] - _roomInfo[i, (int)RoomStatus.X]) - Offset);
                _roomInfo[i, (int)RoomStatus.Rh] = UnityEngine.Random.Range(_roomMin, _roomInfo[i, (int)RoomStatus.H] - (_roomInfo[i, (int)RoomStatus.Ry] - _roomInfo[i, (int)RoomStatus.Y]) - Offset);
            }

            // マップ上書き
            for (var count = 0; count < _roomNum; count++)
            {
                // 取得した部屋の確認
                for (var h = 0; h < _roomInfo[count, (int)RoomStatus.H]; h++)
                {
                    for (var w = 0; w < _roomInfo[count, (int)RoomStatus.W]; w++)
                    {
                        // 部屋チェックポイント
                        _mapKind[w + _roomInfo[count, (int)RoomStatus.X], h + _roomInfo[count, (int)RoomStatus.Y]] = (int)ObjectType.Wall;
                    }

                }

                // 生成した部屋
                for (var h = 0; h < _roomInfo[count, (int)RoomStatus.Rh]; h++)
                {
                    for (var w = 0; w < _roomInfo[count, (int)RoomStatus.RW]; w++)
                    {
                        _mapKind[w + _roomInfo[count, (int)RoomStatus.Rx], h + _roomInfo[count, (int)RoomStatus.Ry]] = (int)ObjectType.Ground;
                    }
                }
            }

            // 道の生成
            var splitLength = new int[4];

            // 部屋から一番近い境界線を調べる(十字に調べる)
            for (var nowRoom = 0; nowRoom < _roomNum; nowRoom++)
            {
                // 左の壁からの距離
                splitLength[0] = _roomInfo[nowRoom, (int)RoomStatus.X] > 0 ?
                    _roomInfo[nowRoom, (int)RoomStatus.Rx] - _roomInfo[nowRoom, (int)RoomStatus.X] : int.MaxValue;
                // 右の壁からの距離
                splitLength[1] = (_roomInfo[nowRoom, (int)RoomStatus.X] + _roomInfo[nowRoom, (int)RoomStatus.W]) < _mapSize ?
                    (_roomInfo[nowRoom, (int)RoomStatus.X] + _roomInfo[nowRoom, (int)RoomStatus.W]) - (_roomInfo[nowRoom, (int)RoomStatus.Rx] + _roomInfo[nowRoom, (int)RoomStatus.RW]) : int.MaxValue;

                // 下の壁からの距離
                splitLength[2] = _roomInfo[nowRoom, (int)RoomStatus.Y] > 0 ?
                    _roomInfo[nowRoom, (int)RoomStatus.Ry] - _roomInfo[nowRoom, (int)RoomStatus.Y] : int.MaxValue;
                // 上の壁からの距離
                splitLength[3] = (_roomInfo[nowRoom, (int)RoomStatus.Y] + _roomInfo[nowRoom, (int)RoomStatus.H]) < _mapSize ?
                    (_roomInfo[nowRoom, (int)RoomStatus.Y] + _roomInfo[nowRoom, (int)RoomStatus.H]) - (_roomInfo[nowRoom, (int)RoomStatus.Ry] + _roomInfo[nowRoom, (int)RoomStatus.Rh]) : int.MaxValue;

                // マックスでない物のみ先へ
                for (var j = 0; j < splitLength.Length; j++)
                {
                    if (splitLength[j] == int.MaxValue) continue;
                    // 上下左右判定
                    int roodPoint;// 道を引く場所
                    if (j < 2)
                    {
                        // 道を引く場所を決定
                        roodPoint = UnityEngine.Random.Range(_roomInfo[nowRoom, (int)RoomStatus.Ry] + Offset, _roomInfo[nowRoom, (int)RoomStatus.Ry] + _roomInfo[nowRoom, (int)RoomStatus.Rh] - Offset);

                        // マップに書き込む
                        for (var w = 1; w <= splitLength[j]; w++)
                        {
                            // 左右判定
                            if (j == 0)
                            {
                                // 左
                                _mapKind[(-w) + _roomInfo[nowRoom, (int)RoomStatus.Rx], roodPoint] = (int)ObjectType.Road;
                            }
                            else
                            {
                                // 右
                                _mapKind[w + _roomInfo[nowRoom, (int)RoomStatus.Rx] + _roomInfo[nowRoom, (int)RoomStatus.RW] - Offset, roodPoint] = (int)ObjectType.Road;

                                // 最後
                                if (w == splitLength[j])
                                {
                                    // 一つ多く作る
                                    _mapKind[w + Offset + _roomInfo[nowRoom, (int)RoomStatus.Rx] + _roomInfo[nowRoom, (int)RoomStatus.RW] - Offset, roodPoint] = (int)ObjectType.Road;
                                }
                            }
                        }
                    }
                    else
                    {
                        // 道を引く場所を決定
                        roodPoint = UnityEngine.Random.Range(_roomInfo[nowRoom, (int)RoomStatus.Rx] + Offset, _roomInfo[nowRoom, (int)RoomStatus.Rx] + _roomInfo[nowRoom, (int)RoomStatus.RW] - Offset);

                        // マップに書き込む
                        for (var h = 1; h <= splitLength[j]; h++)
                        {
                            // 上下判定
                            if (j == 2)
                            {
                                // 下
                                _mapKind[roodPoint, (-h) + _roomInfo[nowRoom, (int)RoomStatus.Ry]] = (int)ObjectType.Road;
                            }
                            else
                            {
                                // 上
                                _mapKind[roodPoint, h + _roomInfo[nowRoom, (int)RoomStatus.Ry] + _roomInfo[nowRoom, (int)RoomStatus.Rh] - Offset] = (int)ObjectType.Road;

                                // 最後
                                if (h == splitLength[j])
                                {
                                    // 一つ多く作る
                                    _mapKind[roodPoint, h + Offset + _roomInfo[nowRoom, (int)RoomStatus.Ry] + _roomInfo[nowRoom, (int)RoomStatus.Rh] - Offset] = (int)ObjectType.Road;
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
                for (var roodScan = 0; roodScan < _roomInfo[nowRoom, (int)RoomStatus.W]; roodScan++)
                {
                    // 道を検索
                    if (_mapKind[roodScan + _roomInfo[nowRoom, (int)RoomStatus.X],
                            _roomInfo[nowRoom, (int)RoomStatus.Y]] != (int)ObjectType.Road) continue;
                    // 道の座標セット
                    if (roadVec1 == 0)
                    {
                        // 始点セット
                        roadVec1 = roodScan + _roomInfo[nowRoom, (int)RoomStatus.X];
                    }
                    else
                    {
                        // 終点セット
                        roadVec2 = roodScan + _roomInfo[nowRoom, (int)RoomStatus.X];
                    }
                }
                // 道を引く
                for (var roadSet = roadVec1; roadSet < roadVec2; roadSet++)
                {
                    // 境界線を上書き
                    _mapKind[roadSet, _roomInfo[nowRoom, (int)RoomStatus.Y]] = (int)ObjectType.Road;
                }

                roadVec1 = 0;
                roadVec2 = 0;

                for (var roadScan = 0; roadScan < _roomInfo[nowRoom, (int)RoomStatus.H]; roadScan++)
                {
                    // 道を検索
                    if (_mapKind[_roomInfo[nowRoom, (int)RoomStatus.X],
                            roadScan + _roomInfo[nowRoom, (int)RoomStatus.Y]] != (int)ObjectType.Road) continue;
                    // 道の座標セット
                    if (roadVec1 == 0)
                    {
                        // 始点セット
                        roadVec1 = roadScan + _roomInfo[nowRoom, (int)RoomStatus.Y];
                    }
                    else
                    {
                        // 終点セット
                        roadVec2 = roadScan + _roomInfo[nowRoom, (int)RoomStatus.Y];
                    }
                }
                // 道を引く
                for (var roadSet = roadVec1; roadSet < roadVec2; roadSet++)
                {
                    // 境界線を上書き
                    _mapKind[_roomInfo[nowRoom, (int)RoomStatus.X], roadSet] = (int)ObjectType.Road;
                }
            }

            // 親オブジェクトの生成
            var groundParent = new GameObject("Ground");
            var wallParent = new GameObject("Wall");
            var roadParent = new GameObject("Road");

            // 配列にプレハブを入れる
            var objectParents = new [] { groundParent, wallParent, roadParent };

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
                            Quaternion.identity,objectParents[_mapKind[nowW, nowH]].transform);
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
                            Quaternion.identity, objectParents[_mapKind[nowW, nowH]].transform);
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
                            Quaternion.identity,objectParents[_mapKind[nowW, nowH]].transform);
                    }

                }
            }
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
        
        // 部屋の中心座標を計算するメソッド
        public Vector3 GetRoomCenters(int roomNum)
        {
            var value = Vector3.zero;
                var centerX = _roomInfo[roomNum, (int)RoomStatus.Rx] + _roomInfo[roomNum, (int)RoomStatus.RW] / 2f;
                var centerZ = _roomInfo[roomNum, (int)RoomStatus.Ry] + _roomInfo[roomNum, (int)RoomStatus.Rh] / 2f;

                value.x = _defaultPosition.x + centerX * _mapObjects[(int)ObjectType.Ground].transform.localScale.x;
                value.y = _defaultPosition.y + _mapObjects[(int)ObjectType.Ground].transform.localScale.y;
                value.z = _defaultPosition.z + centerZ * _mapObjects[(int)ObjectType.Ground].transform.localScale.z;
            return value;
        }
    }
}