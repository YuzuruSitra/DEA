using Item;
using Manager;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ResultPanelHandler : MonoBehaviour
    {
        [SerializeField] private string[] _resultString;
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private TextMeshProUGUI _bornCountText;
        
        private void Start()
        {
            var dungeonLayerHandler = GameObject.FindWithTag("DungeonLayerHandler").GetComponent<DungeonLayerHandler>();
            _resultText.text = dungeonLayerHandler.IsGameClear ? _resultString[0] : _resultString[1];
            var inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            _bornCountText.text = "" + inventoryHandler.ItemSets[(int)ItemKind.Born]._count;
        }
    }
}
