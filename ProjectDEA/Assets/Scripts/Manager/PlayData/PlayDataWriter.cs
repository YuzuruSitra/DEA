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
                SecurityMode = SecurityMode.None,
                Password = "Password",
                CompressionMode = CompressionMode.None
            };
            
        }

        // プレイヤーIDに応じたスロットを設定するメソッド
        public void SetPlayerSlot(int playerId)
        {
            // プレイヤーIDに応じたスロットキーを設定
            _currentSlotKey = $"Player_{playerId}";
            _writer = QuickSaveWriter.Create(_currentSlotKey, _settings);
            
            UnityEngine.Debug.Log($"Player Slot changed to: {_currentSlotKey}");
        }
        // メタAIの使用可否の保存
        public void SaveIsUseMetaAI(string key, bool isUse)
        {
            UnityEngine.Debug.Log(key + " " + isUse);
            _writer.Write(key, isUse);
            _writer.Commit();
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
        // アクションの回数保存
        public void SaveActionCount(string key, float movementCount)
        {
            UnityEngine.Debug.Log(key + " " + movementCount);
            _writer.Write(key, movementCount);
            _writer.Commit();
        }
        // プレイヤータイプの保存
        public void SavePlayerType(string key, string type)
        {
            UnityEngine.Debug.Log(key + " " + type);
            _writer.Write(key, type);
            _writer.Commit();
        }
    }
}
