using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PanelSwitcher : MonoBehaviour
    {
        [SerializeField] private Button _openInventoryBt;
        [SerializeField] private Button _closeInventoryBt;
        [SerializeField] private GameObject _inventoryPanel;

        void Start()
        {
            _openInventoryBt.onClick.AddListener(OpenInventoryPanel);
            _closeInventoryBt.onClick.AddListener(CloseInventoryPanel);
        }

        private void OpenInventoryPanel()
        {
            _inventoryPanel.SetActive(true);
        }

        private void CloseInventoryPanel()
        {
            _inventoryPanel.SetActive(false);
        }
    }
}
