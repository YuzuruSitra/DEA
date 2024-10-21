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
        
        private void Start()
        {
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            SetInventoryFrame(_inventoryHandler.ItemSets);
            _inventoryHandler.OnItemSpriteChanged += ChangeItemImage;
            _inventoryHandler.OnItemCountChanged += ChangeItemCount;
            _inventoryHandler.OnKeyCountChanged += ChangeKeyCount;
            _inventoryHandler.OnItemLineupChanged += ChangeVisibleFrame;
            ChangeKeyCount(_inventoryHandler.ItemSets[(int)ItemKind.Key]._count);
        }

        private void OnDestroy()
        {
            _inventoryHandler.OnItemSpriteChanged -= ChangeItemImage;
            _inventoryHandler.OnItemCountChanged -= ChangeItemCount;
            _inventoryHandler.OnKeyCountChanged -= ChangeKeyCount;
            _inventoryHandler.OnItemLineupChanged -= ChangeVisibleFrame;
        }

        private void SetInventoryFrame(InventoryHandler.ItemPrefabSet[] itemSet)
        {
            _itemFrames = new GameObject[itemSet.Length];
            for (var i = 0; i < itemSet.Length; i++)
            {
                var imageObj = Instantiate(_itemImagePrefab, _inventoryContent.transform, true);
                _itemFrames[i] = imageObj;
                var cd = imageObj.transform.GetChild(0).gameObject;
                var gcd = cd.transform.GetChild(0).gameObject;
                var image = gcd.GetComponent<Image>();
                image.sprite = itemSet[i]._sprite;
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
