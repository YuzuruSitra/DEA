using UnityEngine;
using TMPro;

public class LayerUIHandler : MonoBehaviour
{
    private TextMeshProUGUI _layerText;
    private DungeonLayerHandler _layerHandler;

    void Start()
    {
        _layerText = GetComponent<TextMeshProUGUI>();
        _layerHandler = GameObject.FindWithTag("DungeonLayerHandler").GetComponent<DungeonLayerHandler>();
        _layerHandler.ChangeLayer += ChangeLayerText;
    }

    private void OnDestroy()
    {
        _layerHandler.ChangeLayer -= ChangeLayerText;
    }

    private void ChangeLayerText(int layer)
    {
        _layerText.text = layer + "F";
    }
}
