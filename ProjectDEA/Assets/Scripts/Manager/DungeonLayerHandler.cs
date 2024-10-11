using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonLayerHandler : MonoBehaviour
{
    private int _currentLayer;
    public Action<int> ChangeLayer;

    private void Awake()
    {
        var target = GameObject.FindGameObjectWithTag("DungeonLayerHandler");
        bool checkResult = target != null && target != gameObject;

        if (checkResult)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ChangeLayerCount(_currentLayer);
    }

    public void NextDungeonLayer()
    {
        ChangeLayerCount(_currentLayer++);
        SceneManager.LoadScene("MainScene");
    }

    private void ChangeLayerCount(int layer)
    {
        _currentLayer = layer;
        ChangeLayer?.Invoke(layer);
    }
}
