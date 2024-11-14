using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Manager.MetaAI
{
    public class PlayerTypeClassifier : MonoBehaviour
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private const string PythonServerIP = "127.0.0.1";
        private const int PythonServerPort = 5000;
        [Header("一度に送るログ数")]
        [SerializeField] private int _logPerSend;
        private readonly List<string> _actionLogs = new();

        private void Start()
        {
            // Pythonサーバーへ接続
            ConnectToPythonServer();
        }

        private void Update()
        {
            var killer = 6;
            var achiever = 6;
            var explorer = 6;
            if (Input.GetKeyDown(KeyCode.A))
            {
                killer = 10;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                achiever = 10;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                explorer = 10;
            }

            if (Input.anyKeyDown)
            {
                CollectActionLog(killer, achiever, explorer);
            }
        }

        private void ConnectToPythonServer()
        {
            try
            {
                _client = new TcpClient(PythonServerIP, PythonServerPort);
                _stream = _client.GetStream();
                UnityEngine.Debug.Log("Connected to Python server");
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Connection error: {e.Message}");
            }
        }

        private void CollectActionLog(int killer, int achiever, int explorer)
        {
            // 行動ログを収集
            var log = $"Killer: {killer}, Achiever: {achiever}, Explorer: {explorer}";
            UnityEngine.Debug.Log(log);
        
            _actionLogs.Add(log);

            // ログが10個溜まったら送信
            if (_actionLogs.Count < 10) return;
            SendDataToPython();
            _actionLogs.Clear(); // ログをクリア
        }

        private async void SendDataToPython()
        {
            if (_client is not { Connected: true }) return;

            try
            {
                // 行動ログデータをシリアライズして送信
                var data = string.Join(";", _actionLogs);
                var dataBytes = Encoding.UTF8.GetBytes(data);
                await _stream.WriteAsync(dataBytes, 0, dataBytes.Length);

                // Pythonからのレスポンス受信
                var buffer = new byte[1024];
                var bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                UnityEngine.Debug.Log($"Player Type Prediction: {response}");
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Data send/receive error: {e.Message}");
            }
        }

        private void OnApplicationQuit()
        {
            // 終了時に接続を閉じる
            if (_stream != null) _stream.Close();
            if (_client != null) _client.Close();
        }
    }
}