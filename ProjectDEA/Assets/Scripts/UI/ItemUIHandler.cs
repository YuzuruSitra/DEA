using Player;
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

        public void SetInventoryFrame(PlayerInventory.ItemPrefabSet[] itemSet)
        {
            foreach (var t in itemSet)
            {
                var imageObj = Instantiate(_itemImagePrefab, _inventoryContent.transform, true);
                var cd = imageObj.transform.GetChild(0).gameObject;
                var gcd = cd.transform.GetChild(0).gameObject;
                var image = gcd.GetComponent<Image>();
                image.sprite = t._sprite;
            }
        }

        public void ChangeItemImage(Sprite sprite)
        {
            _itemImage.sprite = sprite;
        }

        public void ChangeItemCount(int value)
        {
            if (value == 0)
            {
                _itemCountText.text = "---";
            }
            else
            {
                _itemCountText.text = "×" + value;
            }
        }
    }
}
