using Manager;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LayerUIHandler : MonoBehaviour
    {
        private TextMeshProUGUI _layerText;
        
        private void Start()
        {
            _layerText = GetComponent<TextMeshProUGUI>();
            var layerHandler = GameObject.FindWithTag("DungeonLayerHandler").GetComponent<DungeonLayerHandler>();
            ChangeLayerText(layerHandler.CurrentLayer);
        }

        private void ChangeLayerText(int layer)
        {
            _layerText.text = layer + "F";
        }
    }
}
