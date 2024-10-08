using UnityEngine;

namespace UI
{
    public class PanelSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject _inventoryPanel;

        public void OpenInventoryPanel()
        {
            _inventoryPanel.SetActive(true);
        }

        public void CloseInventoryPanel()
        {
            _inventoryPanel.SetActive(false);
        }
    }
}
