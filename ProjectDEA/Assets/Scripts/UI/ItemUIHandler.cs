using Manager;
using Manager.Language;
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
        private InventoryHandler _inventoryHandler;
        [SerializeField] private PanelSwitcher _panelSwitcher;
        
        private void Start()
        {
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            SetInventoryFrame(_inventoryHandler.ItemSets);
            _inventoryHandler.OnItemSpriteChanged += ChangeItemImage;
            _inventoryHandler.OnItemCountChanged += ChangeItemCount;
            _inventoryHandler.OnItemLineupChanged += ChangeVisibleFrame;
            _panelSwitcher.IsOpenInventory += ChangeInventoryCount;
        }

        private void OnDestroy()
        {
            _inventoryHandler.OnItemSpriteChanged -= ChangeItemImage;
            _inventoryHandler.OnItemCountChanged -= ChangeItemCount;
            _inventoryHandler.OnItemLineupChanged -= ChangeVisibleFrame;
            _panelSwitcher.IsOpenInventory -= ChangeInventoryCount;
        }

        private void SetInventoryFrame(InventoryHandler.ItemPrefabSet[] itemSet)
        {
            var languageHandler = GameObject.FindWithTag("LanguageHandler").GetComponent<LanguageHandler>();
            _itemFrames = new GameObject[itemSet.Length];
            for (var i = 0; i < itemSet.Length; i++)
            {
                var imageObj = Instantiate(_itemImagePrefab, _inventoryContent.transform, true);
                _itemFrames[i] = imageObj;
                var cd1 = imageObj.transform.GetChild(0).gameObject;
                var nameText = cd1.GetComponent<TextMeshProUGUI>();
                nameText.text = itemSet[i]._name[(int)languageHandler.CurrentLanguage];
                
                var cd2 = imageObj.transform.GetChild(1).gameObject;
                var descriptionText = cd2.GetComponent<TextMeshProUGUI>();
                descriptionText.text = itemSet[i]._description[(int)languageHandler.CurrentLanguage];

                var cd3 = imageObj.transform.GetChild(2).gameObject;
                var countText = cd3.GetComponent<TextMeshProUGUI>();
                if (itemSet[i]._count == InventoryHandler.InfiniteNum)
                {
                    countText.text = "×∞";
                }
                else
                {
                    countText.text = "×" + itemSet[i]._count;   
                }
                var cd4 = imageObj.transform.GetChild(3).gameObject;
                var frame = cd4.transform.GetChild(0).gameObject;
                var image = frame.transform.GetChild(0).gameObject.GetComponent<Image>();
                image.sprite = itemSet[i]._sprite;
                
                if (itemSet[i]._count == InventoryHandler.InfiniteNum || itemSet[i]._count >= 1)
                {
                    _itemFrames[i].SetActive(true);
                }
            }
        }

        private void ChangeInventoryCount()
        {
            for (var i = 0; i < _inventoryHandler.ItemSets.Length; i++)
            {   
                var count =  _itemFrames[i].transform.GetChild(2).gameObject;
                var countText = count.GetComponent<TextMeshProUGUI>();
                var itemCount = _inventoryHandler.ItemSets[i]._count;
                if (itemCount == InventoryHandler.InfiniteNum)
                {
                    countText.text = "×∞";
                    continue;
                }
                countText.text = "×" + _inventoryHandler.ItemSets[i]._count;
            }
        }

        private void ChangeVisibleFrame(InventoryHandler.ItemPrefabSet[] itemSet)
        {
            for (var i = 0; i < itemSet.Length; i++)
            {
                if (itemSet[i]._count == InventoryHandler.InfiniteNum)
                {
                    _itemFrames[i].SetActive(true);
                    continue;
                }
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
            _itemCountText.text = value switch
            {
                0 => "-",
                InventoryHandler.InfiniteNum => "×∞",
                _ => "×" + value
            };
        }
        
    }
}
