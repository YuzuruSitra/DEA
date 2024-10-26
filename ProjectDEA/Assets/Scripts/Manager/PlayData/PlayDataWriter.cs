using System.Collections.Generic;
using CI.QuickSave;
using Gimmick;
using UnityEngine;

namespace Manager.PlayData
{
    public class PlayDataWriter
    {
        private QuickSaveWriter _writer;
        private readonly QuickSaveSettings _settings;
        private string _currentSlotKey;

        public PlayDataWriter()
        {
#if UNITY_EDITOR
            QuickSaveGlobalSettings.StorageLocation = Application.dataPath + "/SaveData";
#else
            QuickSaveGlobalSettings.StorageLocation = Application.persistentDataPath + "/SaveData";
#endif
            _settings = new QuickSaveSettings
            {
                SecurityMode = SecurityMode.Aes,
                Password = "Password",
                CompressionMode = CompressionMode.Gzip
            };
            
        }

        // プレイヤーIDに応じたスロットを設定するメソッド
        public void SetPlayerSlot(int playerId)
        {
            // プレイヤーIDに応じたスロットキーを設定
            _currentSlotKey = $"SaveData_Player_{playerId}";
            _writer = QuickSaveWriter.Create(_currentSlotKey, _settings);
            
            UnityEngine.Debug.Log($"Player Slot changed to: {_currentSlotKey}");
        }

        // クリア判定の保存
        public void SaveIsClear(string key, bool isClear)
        {
            UnityEngine.Debug.Log(key + " " + isClear);
            _writer.Write(key, isClear);
            _writer.Commit();
        }
        // クリアタイムの保存
        public void SaveClearTime(string key, float clearTime)
        {
            UnityEngine.Debug.Log(key + " " + clearTime);
            _writer.Write(key, clearTime);
            _writer.Commit();
        }
        // 部屋移動回数の保存
        public void SaveMovementCount(string key, float movementCount)
        {
            UnityEngine.Debug.Log(key + " " + movementCount);
            _writer.Write(key, movementCount);
            _writer.Commit();
        }
        // ギミックの生成数の保存
        public void SaveGimmicksCount(string key, Dictionary<GimmickGenerator.GimmickKind, int> gimmicksCount)
        {
            foreach (var gimmick in gimmicksCount)
            {
                // gimmick.Key.ToString() + keyをキーとして保存
                var combinedKey = gimmick.Key + "_" + key;
                _writer.Write(combinedKey, gimmick.Value);
                UnityEngine.Debug.Log(combinedKey + " " + gimmick.Value);
            }
            _writer.Commit();
        }
        // 敵撃破回数の保存
        public void SaveEnemyKillCount(string key, int killCount)
        {
            UnityEngine.Debug.Log(key + " " + killCount);
            _writer.Write(key, killCount);
            _writer.Commit();
        }
        // 敵撃破回数の保存
        public void SavePickedBonesCount(string key, int bonesCount)
        {
            UnityEngine.Debug.Log(key + " " + bonesCount);
            _writer.Write(key, bonesCount);
            _writer.Commit();
        }
        // 破壊したオブジェクト数の保存
        public void SaveDestroyObjCount(string key, int destroyCount)
        {
            UnityEngine.Debug.Log(key + " " + destroyCount);
            _writer.Write(key, destroyCount);
            _writer.Commit();
        }
    }
}
