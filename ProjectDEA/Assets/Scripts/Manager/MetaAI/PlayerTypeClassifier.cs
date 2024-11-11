using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class PlayerTypeClassifier : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private const string pythonServerIP = "127.0.0.1";
    private const int pythonServerPort = 5000;

    private List<string> actionLogs = new List<string>();

    void Start()
    {
        // Pythonサーバーへ接続
        ConnectToPythonServer();
    }

    private void Update()
    {
        // var killer = UnityEngine.Random.Range(0, 11);
        // var achiever = UnityEngine.Random.Range(0, 11);
        // var explorer = UnityEngine.Random.Range(0, 11);
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
            CollectActionLog(killer, achiever, explorer, (int)Time.time);
        }
    }

    void ConnectToPythonServer()
    {
        try
        {
            client = new TcpClient(pythonServerIP, pythonServerPort);
            stream = client.GetStream();
            Debug.Log("Connected to Python server");
        }
        catch (Exception e)
        {
            Debug.LogError($"Connection error: {e.Message}");
        }
    }

    void CollectActionLog(int killer, int achiever, int explorer, int timeElapsed)
    {
        // 行動ログを収集
        string log = $"Killer: {killer}, Achiever: {achiever}, Explorer: {explorer}, Time: {timeElapsed}";
        Debug.Log(log);
        
        actionLogs.Add(log);

        // ログが10個溜まったら送信
        if (actionLogs.Count >= 10)
        {
            SendDataToPython();
            actionLogs.Clear(); // ログをクリア
        }
    }

    async void SendDataToPython()
    {
        if (client == null || !client.Connected) return;

        try
        {
            // 行動ログデータをシリアライズして送信
            string data = string.Join(";", actionLogs);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            await stream.WriteAsync(dataBytes, 0, dataBytes.Length);

            // Pythonからのレスポンス受信
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Debug.Log($"Player Type Prediction: {response}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Data send/receive error: {e.Message}");
        }
    }

    void OnApplicationQuit()
    {
        // 終了時に接続を閉じる
        if (stream != null) stream.Close();
        if (client != null) client.Close();
    }
}
