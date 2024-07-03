using UnityEngine;
using Random = UnityEngine.Random;

public class StageGenerator : MonoBehaviour
{
    [SerializeField] Vector3 defaultPosition;
    // Size of one side of the map.
    [SerializeField] private int _mapSize;
    // Block kind.
    private int[,] _mapKind;

    // Number of rooms.
    [SerializeField] private int roomNum;
    // Minimum room size.
    [SerializeField] private int roomMin = 4;
    private int roomCount;                      // 部屋カウント
    private int line = 0;                       // 分割点
    private int[,] roomStatus;                  // 部屋の管理配列

    private enum RoomStatus                     // 部屋の配列ステータス
    {
        x,// マップ座標ｘ
        y,// マップ座標ｙ
        w,// 分割した幅
        h,// 分割した高さ

        rx,// 部屋の生成位置
        ry,// 部屋の生成位置
        rw,// 部屋の幅
        rh,// 部屋の高さ
    }

    enum objectType
    {
        ground = 0,
        wall = 1,
        road = 2,
    }
    
    [SerializeField] private GameObject[] mapObjects;               // マップ生成用のオブジェクト配列

    private const int offsetWall = 2;   // 壁から離す距離
    private const int offset = 1;       // 調整用

    void Start()
    {
        MapGenerate();
    }

    private void MapGenerate()
    {
        // 部屋（StartX、StartY、幅、高さ）
        roomStatus = new int[roomNum, System.Enum.GetNames(typeof(RoomStatus)).Length];

        // フロア設定
        _mapKind = new int[_mapSize, _mapSize];


        // フロアの初期化
        for (int nowW = 0; nowW < _mapSize; nowW++)
        {
            for (int nowH = 0; nowH < _mapSize; nowH++)
            {
                // 壁を貼る
                _mapKind[nowW, nowH] = 2;
            }
        }

        // フロアを入れる
        roomStatus[roomCount, (int)RoomStatus.x] = 0;
        roomStatus[roomCount, (int)RoomStatus.y] = 0;
        roomStatus[roomCount, (int)RoomStatus.w] = _mapSize;
        roomStatus[roomCount, (int)RoomStatus.h] = _mapSize;

        // カウント追加
        roomCount++;

        // 部屋の数だけ分割する
        int parentNum = 0;
        int max = 0; 
        for (int splitNum = 0; splitNum < roomNum - 1; splitNum++)
        {
            // 変数初期化
            parentNum = 0;  // 分割する部屋番号
            max = 0;        // 最大面積

            // 最大の部屋番号を調べる
            for (int maxCheck = 0; maxCheck < roomNum; maxCheck++)
            {
                // 面積比較
                if (max < roomStatus[maxCheck, (int)RoomStatus.w] * roomStatus[maxCheck, (int)RoomStatus.h])
                {
                    // 最大面積上書き
                    max = roomStatus[maxCheck, (int)RoomStatus.w] * roomStatus[maxCheck, (int)RoomStatus.h];

                    // 親の部屋番号セット
                    parentNum = maxCheck;
                }
            }

            // 取得した部屋をさらに割る
            if (SplitPoint(roomStatus[parentNum, (int)RoomStatus.w], roomStatus[parentNum, (int)RoomStatus.h]))
            {
                // 取得
                roomStatus[roomCount, (int)RoomStatus.x] = roomStatus[parentNum, (int)RoomStatus.x];
                roomStatus[roomCount, (int)RoomStatus.y] = roomStatus[parentNum, (int)RoomStatus.y];
                roomStatus[roomCount, (int)RoomStatus.w] = roomStatus[parentNum, (int)RoomStatus.w] - line;
                roomStatus[roomCount, (int)RoomStatus.h] = roomStatus[parentNum, (int)RoomStatus.h];

                // 親の部屋を整形する
                roomStatus[parentNum, (int)RoomStatus.x] += roomStatus[roomCount, (int)RoomStatus.w];
                roomStatus[parentNum, (int)RoomStatus.w] -= roomStatus[roomCount, (int)RoomStatus.w];
            }
            else
            {
                // 取得
                roomStatus[roomCount, (int)RoomStatus.x] = roomStatus[parentNum, (int)RoomStatus.x];
                roomStatus[roomCount, (int)RoomStatus.y] = roomStatus[parentNum, (int)RoomStatus.y];
                roomStatus[roomCount, (int)RoomStatus.w] = roomStatus[parentNum, (int)RoomStatus.w];
                roomStatus[roomCount, (int)RoomStatus.h] = roomStatus[parentNum, (int)RoomStatus.h] - line;

                // 親の部屋を整形する
                roomStatus[parentNum, (int)RoomStatus.y] += roomStatus[roomCount, (int)RoomStatus.h];
                roomStatus[parentNum, (int)RoomStatus.h] -= roomStatus[roomCount, (int)RoomStatus.h];
            }
            // カウントを加算
            roomCount++;
        }

        // 分割した中にランダムな大きさの部屋を生成
        for (int i = 0; i < roomNum; i++)
        {
            // 生成座標の設定
            roomStatus[i, (int)RoomStatus.rx] = Random.Range(roomStatus[i, (int)RoomStatus.x] + offsetWall, (roomStatus[i, (int)RoomStatus.x] + roomStatus[i, (int)RoomStatus.w]) - (roomMin + offsetWall));
            roomStatus[i, (int)RoomStatus.ry] = Random.Range(roomStatus[i, (int)RoomStatus.y] + offsetWall, (roomStatus[i, (int)RoomStatus.y] + roomStatus[i, (int)RoomStatus.h]) - (roomMin + offsetWall));

            // 部屋の大きさを設定
            roomStatus[i, (int)RoomStatus.rw] = Random.Range(roomMin, roomStatus[i, (int)RoomStatus.w] - (roomStatus[i, (int)RoomStatus.rx] - roomStatus[i, (int)RoomStatus.x]) - offset);
            roomStatus[i, (int)RoomStatus.rh] = Random.Range(roomMin, roomStatus[i, (int)RoomStatus.h] - (roomStatus[i, (int)RoomStatus.ry] - roomStatus[i, (int)RoomStatus.y]) - offset);
        }

        // マップ上書き
        for (int count = 0; count < roomNum; count++)
        {
            // 取得した部屋の確認
            for (int h = 0; h < roomStatus[count, (int)RoomStatus.h]; h++)
            {
                for (int w = 0; w < roomStatus[count, (int)RoomStatus.w]; w++)
                {
                    // 部屋チェックポイント
                    _mapKind[w + roomStatus[count, (int)RoomStatus.x], h + roomStatus[count, (int)RoomStatus.y]] = (int)objectType.wall;
                }

            }

            // 生成した部屋
            for (int h = 0; h < roomStatus[count, (int)RoomStatus.rh]; h++)
            {
                for (int w = 0; w < roomStatus[count, (int)RoomStatus.rw]; w++)
                {
                    _mapKind[w + roomStatus[count, (int)RoomStatus.rx], h + roomStatus[count, (int)RoomStatus.ry]] = (int)objectType.ground;
                }
            }
        }

        // 道の生成
        int[] splitLength = new int[4];
        int roodPoint = 0;// 道を引く場所

        // 部屋から一番近い境界線を調べる(十字に調べる)
        for (int nowRoom = 0; nowRoom < roomNum; nowRoom++)
        {
            // 左の壁からの距離
            splitLength[0] = roomStatus[nowRoom, (int)RoomStatus.x] > 0 ?
                roomStatus[nowRoom, (int)RoomStatus.rx] - roomStatus[nowRoom, (int)RoomStatus.x] : int.MaxValue;
            // 右の壁からの距離
            splitLength[1] = (roomStatus[nowRoom, (int)RoomStatus.x] + roomStatus[nowRoom, (int)RoomStatus.w]) < _mapSize ?
                (roomStatus[nowRoom, (int)RoomStatus.x] + roomStatus[nowRoom, (int)RoomStatus.w]) - (roomStatus[nowRoom, (int)RoomStatus.rx] + roomStatus[nowRoom, (int)RoomStatus.rw]) : int.MaxValue;

            // 下の壁からの距離
            splitLength[2] = roomStatus[nowRoom, (int)RoomStatus.y] > 0 ?
                roomStatus[nowRoom, (int)RoomStatus.ry] - roomStatus[nowRoom, (int)RoomStatus.y] : int.MaxValue;
            // 上の壁からの距離
            splitLength[3] = (roomStatus[nowRoom, (int)RoomStatus.y] + roomStatus[nowRoom, (int)RoomStatus.h]) < _mapSize ?
                (roomStatus[nowRoom, (int)RoomStatus.y] + roomStatus[nowRoom, (int)RoomStatus.h]) - (roomStatus[nowRoom, (int)RoomStatus.ry] + roomStatus[nowRoom, (int)RoomStatus.rh]) : int.MaxValue;

            // マックスでない物のみ先へ
            for (int j = 0; j < splitLength.Length; j++)
            {
                if (splitLength[j] != int.MaxValue)
                {
                    // 上下左右判定
                    if (j < 2)
                    {
                        // 道を引く場所を決定
                        roodPoint = Random.Range(roomStatus[nowRoom, (int)RoomStatus.ry] + offset, roomStatus[nowRoom, (int)RoomStatus.ry] + roomStatus[nowRoom, (int)RoomStatus.rh] - offset);

                        // マップに書き込む
                        for (int w = 1; w <= splitLength[j]; w++)
                        {
                            // 左右判定
                            if (j == 0)
                            {
                                // 左
                                _mapKind[(-w) + roomStatus[nowRoom, (int)RoomStatus.rx], roodPoint] = (int)objectType.road;
                            }
                            else
                            {
                                // 右
                                _mapKind[w + roomStatus[nowRoom, (int)RoomStatus.rx] + roomStatus[nowRoom, (int)RoomStatus.rw] - offset, roodPoint] = (int)objectType.road;

                                // 最後
                                if (w == splitLength[j])
                                {
                                    // 一つ多く作る
                                    _mapKind[w + offset + roomStatus[nowRoom, (int)RoomStatus.rx] + roomStatus[nowRoom, (int)RoomStatus.rw] - offset, roodPoint] = (int)objectType.road;
                                }
                            }
                        }
                    }
                    else
                    {
                        // 道を引く場所を決定
                        roodPoint = Random.Range(roomStatus[nowRoom, (int)RoomStatus.rx] + offset, roomStatus[nowRoom, (int)RoomStatus.rx] + roomStatus[nowRoom, (int)RoomStatus.rw] - offset);

                        // マップに書き込む
                        for (int h = 1; h <= splitLength[j]; h++)
                        {
                            // 上下判定
                            if (j == 2)
                            {
                                // 下
                                _mapKind[roodPoint, (-h) + roomStatus[nowRoom, (int)RoomStatus.ry]] = (int)objectType.road;
                            }
                            else
                            {
                                // 上
                                _mapKind[roodPoint, h + roomStatus[nowRoom, (int)RoomStatus.ry] + roomStatus[nowRoom, (int)RoomStatus.rh] - offset] = (int)objectType.road;

                                // 最後
                                if (h == splitLength[j])
                                {
                                    // 一つ多く作る
                                    _mapKind[roodPoint, h + offset + roomStatus[nowRoom, (int)RoomStatus.ry] + roomStatus[nowRoom, (int)RoomStatus.rh] - offset] = (int)objectType.road;
                                }
                            }
                        }
                    }
                }
            }
        }

        int roadVec1 = 0;// 道の始点
        int roadVec2 = 0;// 道の終点

        // 道の接続
        for (int nowRoom = 0; nowRoom < roomNum; nowRoom++)
        {
            roadVec1 = 0;
            roadVec2 = 0;
            // 道を繋げる
            for (int roodScan = 0; roodScan < roomStatus[nowRoom, (int)RoomStatus.w]; roodScan++)
            {
                // 道を検索
                if (_mapKind[roodScan + roomStatus[nowRoom, (int)RoomStatus.x], roomStatus[nowRoom, (int)RoomStatus.y]] == (int)objectType.road)
                {
                    // 道の座標セット
                    if (roadVec1 == 0)
                    {
                        // 始点セット
                        roadVec1 = roodScan + roomStatus[nowRoom, (int)RoomStatus.x];
                    }
                    else
                    {
                        // 終点セット
                        roadVec2 = roodScan + roomStatus[nowRoom, (int)RoomStatus.x];
                    }
                }
            }
            // 道を引く
            for (int roadSet = roadVec1; roadSet < roadVec2; roadSet++)
            {
                // 境界線を上書き
                _mapKind[roadSet, roomStatus[nowRoom, (int)RoomStatus.y]] = (int)objectType.road;
            }

            roadVec1 = 0;
            roadVec2 = 0;

            for (int roadScan = 0; roadScan < roomStatus[nowRoom, (int)RoomStatus.h]; roadScan++)
            {
                // 道を検索
                if (_mapKind[roomStatus[nowRoom, (int)RoomStatus.x], roadScan + roomStatus[nowRoom, (int)RoomStatus.y]] == (int)objectType.road)
                {
                    // 道の座標セット
                    if (roadVec1 == 0)
                    {
                        // 始点セット
                        roadVec1 = roadScan + roomStatus[nowRoom, (int)RoomStatus.y];
                    }
                    else
                    {
                        // 終点セット
                        roadVec2 = roadScan + roomStatus[nowRoom, (int)RoomStatus.y];
                    }
                }
            }
            // 道を引く
            for (int roadSet = roadVec1; roadSet < roadVec2; roadSet++)
            {
                // 境界線を上書き
                _mapKind[roomStatus[nowRoom, (int)RoomStatus.x], roadSet] = (int)objectType.road;
            }
        }

        // 親オブジェクトの生成
        var groundParent = new GameObject("Ground");
        var wallParent = new GameObject("Wall");
        var roadParent = new GameObject("Road");

        // 配列にプレハブを入れる
        var objectParents = new GameObject[] { groundParent, wallParent, roadParent };

        // オブジェクトを生成する
        for (int nowH = 0; nowH < _mapSize; nowH++)
        {
            for (int nowW = 0; nowW < _mapSize; nowW++)
            {
                // 壁の生成
                if (_mapKind[nowW, nowH] == (int)objectType.wall)
                {
                    GameObject mazeObject = Instantiate(
                        mapObjects[_mapKind[nowW, nowH]],
                        new Vector3(
                            defaultPosition.x + nowW * mapObjects[_mapKind[nowW, nowH]].transform.localScale.x,
                            defaultPosition.y + (mapObjects[(int)objectType.wall].transform.localScale.y - mapObjects[(int)objectType.ground].transform.localScale.y) * 0.5f,
                            defaultPosition.z + nowH * mapObjects[_mapKind[nowW, nowH]].transform.localScale.z),
                        Quaternion.identity,objectParents[_mapKind[nowW, nowH]].transform);
                }

                // 部屋の生成
                if (_mapKind[nowW, nowH] == (int)objectType.ground)
                {
                    GameObject mazeObject = Instantiate(
                        mapObjects[_mapKind[nowW, nowH]],
                        new Vector3(
                            defaultPosition.x + nowW * mapObjects[_mapKind[nowW, nowH]].transform.localScale.x,
                            defaultPosition.y,
                            defaultPosition.z + nowH * mapObjects[_mapKind[nowW, nowH]].transform.localScale.z),
                        Quaternion.identity, objectParents[_mapKind[nowW, nowH]].transform);
                }

                // 通路の生成
                if (_mapKind[nowW, nowH] == (int)objectType.road)
                {
                    GameObject mazeObject = Instantiate(
                        mapObjects[_mapKind[nowW, nowH]],
                        new Vector3(
                            defaultPosition.x + nowW * mapObjects[_mapKind[nowW, nowH]].transform.localScale.x,
                            defaultPosition.y,
                            defaultPosition.z + nowH * mapObjects[_mapKind[nowW, nowH]].transform.localScale.z),
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
            line = Random.Range(roomMin + (offsetWall * 2), x - (offsetWall * 2 + roomMin));// 縦割り
            return true;
        }
        else
        {
            line = Random.Range(roomMin + (offsetWall * 2), y - (offsetWall * 2 + roomMin));// 横割り
            return false;
        }
    }

}