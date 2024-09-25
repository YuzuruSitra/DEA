using UnityEngine;
using System;
using System.Map;
using System.Collections.Generic;

namespace Gimmick
{
    public class GimmickGenerator : MonoBehaviour
    {
        private const int PaddingThreshold = 1;
        public enum GimmickKind
        {
            Water,
            TeresureBox
        }
        [Serializable]
        public struct GimmickInfo
        {
            public GimmickKind Kind;
            public GameObject Prefab;
            public bool IsRandomGenerate;
        }
        [SerializeField]
        private GimmickInfo[] _gimmickInfo;

        public void GenerateGimmick(StageGenerator stageGenerator)
        {
            // 生成対象のギミックをリスト化
            var insList = new List<GameObject>();
            foreach (var gimmick in _gimmickInfo)
            {
                if (gimmick.IsRandomGenerate)
                {
                    insList.Add(gimmick.Prefab);
                }
            }
            // 必要な値のインスタンス
            var groundY = stageGenerator.GroundPosY;
            var roomCount = stageGenerator.RoomCount;
            var roomInfo = stageGenerator.RoomInfo;
            // 全ての部屋にギミックを一つ生成
            for (var i = 0; i < roomCount; i++)
            {
                var insGimmick = insList[ UnityEngine.Random.Range(0, insList.Count) ];
                var paddingX = (int)Math.Ceiling(insGimmick.transform.localScale.x / 2.0f) + PaddingThreshold;
                var paddingZ =  (int)Math.Ceiling(insGimmick.transform.localScale.z / 2.0f) + PaddingThreshold;
                var rangeMinX = roomInfo[i, (int)StageGenerator.RoomStatus.TopLeftX] + paddingX;
                var rangeMaxX = roomInfo[i, (int)StageGenerator.RoomStatus.TopRightX] - paddingX;
                var insPosX = UnityEngine.Random.Range(rangeMinX, rangeMaxX + 1);
                var rangeMinZ = roomInfo[i, (int)StageGenerator.RoomStatus.BottomLeftZ] + paddingZ;
                var rangeMaxZ = roomInfo[i, (int)StageGenerator.RoomStatus.TopLeftZ ] - paddingZ;
                var insPosZ = UnityEngine.Random.Range(rangeMinZ, rangeMaxZ + 1);
                var insPosY = groundY + insGimmick.transform.localScale.y / 2.0f;
                var insPos = SetVector3(insPosX, insPosY, insPosZ);
                Instantiate(insGimmick, insPos, Quaternion.identity);
            }
        }

        private Vector3 SetVector3(int x, float y, int z)
        {
            var insPos = Vector3.zero;
            insPos.x = x;
            insPos.y = y;
            insPos.z = z;
            return insPos;
        }

    }
}