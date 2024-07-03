// using UnityEngine;

// public class ObjectInstantiator
// {
//     private int[,] map;
//     private int mapSizeW;
//     private int mapSizeH;
//     private Vector3 defaultPosition;
//     private GameObject wallPrefab;
//     private GameObject floorPrefab;

//     public ObjectInstantiator(int[,] map, int mapSizeW, int mapSizeH, Vector3 defaultPosition, GameObject wallPrefab, GameObject floorPrefab)
//     {
//         this.map = map;
//         this.mapSizeW = mapSizeW;
//         this.mapSizeH = mapSizeH;
//         this.defaultPosition = defaultPosition;
//         this.wallPrefab = wallPrefab;
//         this.floorPrefab = floorPrefab;
//     }

//     public void InstantiateObjects()
//     {
//         for (int w = 0; w < mapSizeW; w++)
//         {
//             for (int h = 0; h < mapSizeH; h++)
//             {
//                 if (map[w, h] == 2)
//                 {
//                     Object.Instantiate(wallPrefab, new Vector3(w * 10.0f, 0.0f, h * 10.0f) + defaultPosition, Quaternion.identity);
//                 }
//                 else if (map[w, h] == 0)
//                 {
//                     Object.Instantiate(floorPrefab, new Vector3(w * 10.0f, 0.0f, h * 10.0f) + defaultPosition, Quaternion.identity);
//                 }
//             }
//         }
//     }
// }
