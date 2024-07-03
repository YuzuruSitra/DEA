// using UnityEngine;
// using Random = UnityEngine.Random;

// public class RoadGenerator
// {
//     private int[,] map;
//     private int[,] roomStatus;
//     private int roomNum;
//     private int mapSizeW;
//     private int mapSizeH;
//     private const int offset = 1;

//     public RoadGenerator(int[,] map, int[,] roomStatus, int roomNum, int mapSizeW, int mapSizeH)
//     {
//         this.map = map;
//         this.roomStatus = roomStatus;
//         this.roomNum = roomNum;
//         this.mapSizeW = mapSizeW;
//         this.mapSizeH = mapSizeH;
//     }

//     public void GenerateRoads()
//     {
//         for (int roomRoad = 0; roomRoad < roomNum - 1; roomRoad++)
//         {
//             int roomNumStart = 0;
//             int roomNumEnd = 0;
//             int pointXStart = 0;
//             int pointYStart = 0;
//             int pointXEnd = 0;
//             int pointYEnd = 0;

//             if (SplitPoint(roomStatus[2, roomRoad], roomStatus[3, roomRoad]))
//             {
//                 roomNumStart = roomRoad;
//                 roomNumEnd = roomRoad + 1;

//                 pointXStart = Random.Range(roomStatus[4, roomNumStart] + offset, roomStatus[4, roomNumStart] + roomStatus[6, roomNumStart] - offset);
//                 pointYStart = roomStatus[1, roomNumStart] + roomStatus[3, roomNumStart] - offset;

//                 pointXEnd = Random.Range(roomStatus[4, roomNumEnd] + offset, roomStatus[4, roomNumEnd] + roomStatus[6, roomNumEnd] - offset);
//                 pointYEnd = roomStatus[1, roomNumEnd];
//             }
//             else
//             {
//                 roomNumStart = roomRoad + 1;
//                 roomNumEnd = roomRoad;

//                 pointXStart = roomStatus[0, roomNumStart];
//                 pointYStart = Random.Range(roomStatus[5, roomNumStart] + offset, roomStatus[5, roomNumStart] + roomStatus[7, roomNumStart] - offset);

//                 pointXEnd = roomStatus[0, roomNumEnd] + roomStatus[2, roomNumEnd] - offset;
//                 pointYEnd = Random.Range(roomStatus[5, roomNumEnd] + offset, roomStatus[5, roomNumEnd] + roomStatus[7, roomNumEnd] - offset);
//             }

//             int roadXStart = pointXStart;
//             int roadXEnd = pointXEnd;
//             int roadYStart = pointYStart;
//             int roadYEnd = pointYEnd;

//             while (roadXStart != roadXEnd || roadYStart != roadYEnd)
//             {
//                 if (roadXStart != roadXEnd)
//                 {
//                     map[roadXStart, roadYStart] = 0;
//                     if (roadXStart < roadXEnd)
//                     {
//                         roadXStart++;
//                     }
//                     else
//                     {
//                         roadXStart--;
//                     }
//                 }
//                 else
//                 {
//                     map[roadXStart, roadYStart] = 0;
//                     if (roadYStart < roadYEnd)
//                     {
//                         roadYStart++;
//                     }
//                     else
//                     {
//                         roadYStart--;
//                     }
//                 }
//             }
//         }
//     }

//     private bool SplitPoint(int x, int y)
//     {
//         return x > y;
//     }
// }
