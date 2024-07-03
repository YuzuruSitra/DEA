// using UnityEngine;

// public class MapInitializer
// {
//     private int mapSizeW;
//     private int mapSizeH;
//     private GameObject wallPrefab;
//     private GameObject floorPrefab;
//     public int[,] Map { get; private set; }

//     public MapInitializer(int mapSizeW, int mapSizeH, GameObject wallPrefab, GameObject floorPrefab)
//     {
//         this.mapSizeW = mapSizeW;
//         this.mapSizeH = mapSizeH;
//         this.wallPrefab = wallPrefab;
//         this.floorPrefab = floorPrefab;
//         Map = new int[mapSizeW, mapSizeH];
//     }

//     public void InitializeMap()
//     {
//         for (int w = 0; w < mapSizeW; w++)
//         {
//             for (int h = 0; h < mapSizeH; h++)
//             {
//                 Map[w, h] = 2; // •Ç
//             }
//         }
//     }
// }
