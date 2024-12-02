using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Manager.MetaAI
{
    public class PlayerTypeClassifier
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private const string PythonServerIP = "127.0.0.1";
        private const int PythonServerPort = 6000;
        private readonly int _logPerSend;
        private readonly List<string> _actionLogs = new();
        public Action<MetaAIHandler.PlayerType> ResponsePlayerType;
        
        public PlayerTypeClassifier(int logPerSend)
        {
            _logPerSend = logPerSend;
        }

        public void ConnectToPythonServer()
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

        public void CollectActionLog(int killer, int achiever, int explorer)
        {
            var log = $"Killer: {killer}, Achiever: {achiever}, Explorer: {explorer}";
            //UnityEngine.Debug.Log(log);

            _actionLogs.Add(log);

            if (_actionLogs.Count < _logPerSend) return;
            SendAndReceiveDataToPython();
            _actionLogs.Clear();
        }

        private async void SendAndReceiveDataToPython()
        {
            if (_client is not { Connected: true }) return;

            try
            {
                // データ送信
                var data = string.Join(";", _actionLogs);
                var dataBytes = Encoding.UTF8.GetBytes(data);
                await _stream.WriteAsync(dataBytes, 0, dataBytes.Length);

                // データ受信 (Pythonの処理完了を待つ)
                var buffer = new byte[1024];
                var bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    SetResponsePlayerType(response);
                }
                else
                {
                    UnityEngine.Debug.LogWarning("No response received from Python server.");
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Data send/receive error: {e.Message}");
            }
        }

        public async void SendResetToPython()
        {
            // データ送信
            const string data = "RESET";
            var dataBytes = Encoding.UTF8.GetBytes(data);
            await _stream.WriteAsync(dataBytes, 0, dataBytes.Length);
        }

        private void SetResponsePlayerType(string response)
        {
            var responsePlayerType = response switch
            {
                "Killer" => MetaAIHandler.PlayerType.Killer,
                "Achiever" => MetaAIHandler.PlayerType.Achiever,
                "Explorer" => MetaAIHandler.PlayerType.Explorer,
                _ => throw new ArgumentOutOfRangeException(nameof(response), response, null)
            };
            ResponsePlayerType?.Invoke(responsePlayerType);
            UnityEngine.Debug.Log($"Player Type Prediction: {responsePlayerType}");
        }

        public void OnDisconnected()
        {
            if (_stream != null) _stream.Close();
            if (_client != null) _client.Close();
        }
    }
}
