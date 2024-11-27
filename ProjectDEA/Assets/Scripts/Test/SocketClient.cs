using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class SocketClient : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;

    // �T�[�o�[�̃A�h���X�ƃ|�[�g�ԍ�
    public string serverAddress = "127.0.0.1";
    public int serverPort = 6000;

    void Start()
    {
        try
        {
            // �T�[�o�[�ɐڑ�
            client = new TcpClient(serverAddress, serverPort);
            stream = client.GetStream();
            Debug.Log("Connected to server.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error connecting to server: {e.Message}");
        }
    }

    void Update()
    {
        // Enter�L�[�Ń��b�Z�[�W�𑗐M
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendMessageToServer("Hello from Unity!");
        }
    }

    private void SendMessageToServer(string message)
    {
        if (stream == null)
        {
            Debug.LogError("Not connected to server.");
            return;
        }

        try
        {
            // ���b�Z�[�W�𑗐M
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Debug.Log($"Sent: {message}");

            // �T�[�o�[����̃��X�|���X����M
            byte[] responseData = new byte[1024];
            int bytes = stream.Read(responseData, 0, responseData.Length);
            string response = Encoding.UTF8.GetString(responseData, 0, bytes);
            Debug.Log($"Received: {response}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error sending message: {e.Message}");
        }
    }

    void OnApplicationQuit()
    {
        // �ڑ������
        if (stream != null)
            stream.Close();
        if (client != null)
            client.Close();
        Debug.Log("Disconnected from server.");
    }
}
