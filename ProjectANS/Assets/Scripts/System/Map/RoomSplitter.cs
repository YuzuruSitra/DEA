// using UnityEngine;
// using Random = UnityEngine.Random;

// public class RoomSplitter
// {
//     private int[,] map;
//     private int roomNum;
//     private int mapSizeW;
//     private int mapSizeH;
//     public int[,] RoomStatus { get; private set; }
//     private int roomMin = 6;
//     private int roomCount = 0;
//     private int line = 0;
//     private const int offsetWall = 4;

//     public RoomSplitter(int[,] map, int roomNum, int mapSizeW, int mapSizeH)
//     {
//         this.map = map;
//         this.roomNum = roomNum;
//         this.mapSizeW = mapSizeW;
//         this.mapSizeH = mapSizeH;
//         RoomStatus = new int[7, roomNum]; // RoomStatusÇÃÉTÉCÉYÇ…çáÇÌÇπÇƒèâä˙âª
//     }

//     public void SplitRooms()
//     {
//         RoomStatus[0, roomCount] = 0;
//         RoomStatus[1, roomCount] = 0;
//         RoomStatus[2, roomCount] = mapSizeW;
//         RoomStatus[3, roomCount] = mapSizeH;
//         roomCount++;

//         for (int splitNum = 0; splitNum < roomNum - 1; splitNum++)
//         {
//             int parentNum = 0;
//             int max = 0;

//             for (int maxCheck = 0; maxCheck < roomNum; maxCheck++)
//             {
//                 if (max < RoomStatus[2, maxCheck] * RoomStatus[3, maxCheck])
//                 {
//                     max = RoomStatus[2, maxCheck] * RoomStatus[3, maxCheck];
//                     parentNum = maxCheck;
//                 }
//             }

//             if (SplitPoint(RoomStatus[2, parentNum], RoomStatus[3, parentNum]))
//             {
//                 RoomStatus[0, roomCount] = RoomStatus[0, parentNum];
//                 RoomStatus[1, roomCount] = RoomStatus[1, parentNum];
//                 RoomStatus[2, roomCount] = RoomStatus[2, parentNum] - line;
//                 RoomStatus[3, roomCount] = RoomStatus[3, parentNum];

//                 RoomStatus[0, parentNum] += RoomStatus[2, roomCount];
//                 RoomStatus[2, parentNum] -= RoomStatus[2, roomCount];
//             }
//             else
//             {
//                 RoomStatus[0, roomCount] = RoomStatus[0, parentNum];
//                 RoomStatus[1, roomCount] = RoomStatus[1, parentNum];
//                 RoomStatus[2, roomCount] = RoomStatus[2, parentNum];
//                 RoomStatus[3, roomCount] = RoomStatus[3, parentNum] - line;

//                 RoomStatus[1, parentNum] += RoomStatus[3, roomCount];
//                 RoomStatus[3, parentNum] -= RoomStatus[3, roomCount];
//             }
//             roomCount++;
//         }
//     }

//     private bool SplitPoint(int x, int y)
//     {
//         if (x > y)
//         {
//             line = Random.Range(roomMin + (offsetWall * 2), x - (offsetWall * 2 + roomMin));
//             return true;
//         }
//         else
//         {
//             line = Random.Range(roomMin + (offsetWall * 2), y - (offsetWall * 2 + roomMin));
//             return false;
//         }
//     }
// }
