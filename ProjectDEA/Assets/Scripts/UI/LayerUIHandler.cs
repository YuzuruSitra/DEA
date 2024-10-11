using Manager;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LayerUIHandler : MonoBehaviour
    {
        private TextMeshProUGUI _layerText;
        [SerializeField]
        private DungeonLayerHandler _layerHandler;
        
        private void Start()
        {
            _layerText = GetComponent<TextMeshProUGUI>();
            _layerHandler = GameObject.FindWithTag("DungeonLayerHandler").GetComponent<DungeonLayerHandler>();
            ChangeLayerText(_layerHandler.CurrentLayer);
        }

        private void ChangeLayerText(int layer)
        {
            _layerText.text = layer + "F";
        }
    }
}
