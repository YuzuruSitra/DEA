using Player;
using UnityEngine;
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
        [SerializeField] private PlayerInventory _playerInventory;
        
        private void Start()
        {
            _openInventoryBt.onClick.AddListener(_panelSwitcher.OpenInventoryPanel);
            _closeInventoryBt.onClick.AddListener(_panelSwitcher.CloseInventoryPanel);
            _itemIncreaseBt.onClick.AddListener(_playerInventory.IncreaseItemNum);
            _itemDecreaseBt.onClick.AddListener(_playerInventory.DecreaseItemNum);
        }
    }
}
