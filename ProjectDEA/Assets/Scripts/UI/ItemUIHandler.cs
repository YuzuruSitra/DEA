using Item;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ItemUIHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _itemImagePrefab;
        [SerializeField] private GameObject _inventoryContent;
        [SerializeField] private Image _itemImage;
        [SerializeField] private TextMeshProUGUI _itemCountText;
        [SerializeField] private GameObject[] _itemFrames;
        [SerializeField] private TextMeshProUGUI _keyText;
        private InventoryHandler _inventoryHandler;
        [SerializeField] private PanelSwitcher _panelSwitcher;
        
        private void Start()
        {
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            SetInventoryFrame(_inventoryHandler.ItemSets);
            _inventoryHandler.OnItemSpriteChanged += ChangeItemImage;
            _inventoryHandler.OnItemCountChanged += ChangeItemCount;
            _inventoryHandler.OnKeyCountChanged += ChangeKeyCount;
            _inventoryHandler.OnItemLineupChanged += ChangeVisibleFrame;
            ChangeKeyCount(_inventoryHandler.ItemSets[(int)ItemKind.Key]._count);
            _panelSwitcher.IsOpenInventory += ChangeInventoryCount;
        }

        private void OnDestroy()
        {
            _inventoryHandler.OnItemSpriteChanged -= ChangeItemImage;
            _inventoryHandler.OnItemCountChanged -= ChangeItemCount;
            _inventoryHandler.OnKeyCountChanged -= ChangeKeyCount;
            _inventoryHandler.OnItemLineupChanged -= ChangeVisibleFrame;
            _panelSwitcher.IsOpenInventory -= ChangeInventoryCount;
        }

        private void SetInventoryFrame(InventoryHandler.ItemPrefabSet[] itemSet)
        {
            _itemFrames = new GameObject[itemSet.Length];
            for (var i = 0; i < itemSet.Length; i++)
            {
                var imageObj = Instantiate(_itemImagePrefab, _inventoryContent.transform, true);
                _itemFrames[i] = imageObj;
                var cd1 = imageObj.transform.GetChild(0).gameObject;
                var nameText = cd1.GetComponent<TextMeshProUGUI>();
                nameText.text = itemSet[i]._name;
                
                var cd2 = imageObj.transform.GetChild(1).gameObject;
                var descriptionText = cd2.GetComponent<TextMeshProUGUI>();
                descriptionText.text = itemSet[i]._description;
                Debug.Log(descriptionText.text);

                var cd3 = imageObj.transform.GetChild(2).gameObject;
                var countText = cd3.GetComponent<TextMeshProUGUI>();
                countText.text = "×" + itemSet[i]._count;
                
                var cd4 = imageObj.transform.GetChild(3).gameObject;
                var frame = cd4.transform.GetChild(0).gameObject;
                var image = frame.transform.GetChild(0).gameObject.GetComponent<Image>();
                image.sprite = itemSet[i]._sprite;
            }
        }

        private void ChangeInventoryCount()
        {
            for (var i = 0; i < _inventoryHandler.ItemSets.Length; i++)
            {   
                var count =  _itemFrames[i].transform.GetChild(2).gameObject;
                var countText = count.GetComponent<TextMeshProUGUI>();
                countText.text = "×" + _inventoryHandler.ItemSets[i]._count;
            }
        }

        private void ChangeVisibleFrame(InventoryHandler.ItemPrefabSet[] itemSet)
        {
            for (var i = 0; i < itemSet.Length; i++)
            {
                _itemFrames[i].SetActive(itemSet[i]._count > 0);
            }
        }

        private void ChangeItemImage(Sprite sprite)
        {
            if (_itemImage.sprite == sprite) return;
            var color = _itemImage.color;
            color.a = sprite == null ? 0 : 1.0f;
            _itemImage.color = color;
            _itemImage.sprite = sprite;
        }

        private void ChangeItemCount(int value)
        {
            if (value == 0)
            {
                _itemCountText.text = "-";
            }
            else
            {
                _itemCountText.text = "×" + value;
            }
        }

        private void ChangeKeyCount(int value)
        {
            _keyText.text = "× " + value;
        }
        
    }
}
