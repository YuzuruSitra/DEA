// using UnityEngine;
// using Random = UnityEngine.Random;

// public class RoomGenerator
// {
//     private int[,] map;
//     private int[,] roomStatus;
//     private int mapSizeW;
//     private int mapSizeH;
//     private const int offsetWall = 4;
//     private const int roomMin = 6;
//     private const int offset = 1;

//     public RoomGenerator(int[,] map, int[,] roomStatus, int mapSizeW, int mapSizeH)
//     {
//         this.map = map;
//         this.roomStatus = roomStatus;
//         this.mapSizeW = mapSizeW;
//         this.mapSizeH = mapSizeH;
//     }

//     public void GenerateRooms()
//     {
//         for (int i = 0; i < roomStatus.GetLength(1); i++)
//         {
//             roomStatus[4, i] = Random.Range(roomStatus[0, i] + offsetWall, roomStatus[0, i] + roomStatus[2, i] - (roomMin + offsetWall));
//             roomStatus[5, i] = Random.Range(roomStatus[1, i] + offsetWall, roomStatus[1, i] + roomStatus[3, i] - (roomMin + offsetWall));

//             roomStatus[6, i] = Random.Range(roomMin, roomStatus[2, i] - (roomStatus[4, i] - roomStatus[0, i]) - offset);
//             roomStatus[7, i] = Random.Range(roomMin, roomStatus[3, i] - (roomStatus[5, i] - roomStatus[1, i]) - offset);
//         }

//         for (int count = 0; count < roomStatus.GetLength(1); count++)
//         {
//             for (int h = 0; h < roomStatus[3, count]; h++)
//             {
//                 for (int w = 0; w < roomStatus[2, count]; w++)
//                 {
//                     map[w + roomStatus[0, count], h + roomStatus[1, count]] = 1;
//                 }
//             }

//             for (int h = 0; h < roomStatus[7, count]; h++)
//             {
//                 for (int w = 0; w < roomStatus[6, count]; w++)
//                 {
//                     map[w + roomStatus[4, count], h + roomStatus[5, count]] = 0;
//                 }
//             }
//         }
//     }
// }
