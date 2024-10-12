using Manager;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class BtAddListenerHandler : MonoBehaviour
    {
        [SerializeField] private Button _openInventoryBt;
        [SerializeField] private Button _closeInventoryBt;
        [SerializeField] private Button _itemIncreaseBt;
        [SerializeField] private Button _itemDecreaseBt;
        [SerializeField] private PanelSwitcher _panelSwitcher;
        
        private void Start()
        {
            _openInventoryBt.onClick.AddListener(_panelSwitcher.OpenInventoryPanel);
            _closeInventoryBt.onClick.AddListener(_panelSwitcher.CloseInventoryPanel);
            var inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            _itemIncreaseBt.onClick.AddListener(inventoryHandler.IncreaseItemNum);
            _itemDecreaseBt.onClick.AddListener(inventoryHandler.DecreaseItemNum);
        }
    }
}
