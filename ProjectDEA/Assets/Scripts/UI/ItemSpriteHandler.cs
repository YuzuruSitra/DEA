using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ItemSpriteHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _itemImagePrefab;
        [SerializeField] private GameObject _inventoryContent;
        [SerializeField] private Image _itemImage;

        public void SetInventoryFrame(PlayerInventory.ItemPrefabSet[] itemSet)
        {
            foreach (var t in itemSet)
            {
                var imageObj = Instantiate(_itemImagePrefab, _inventoryContent.transform, true);
                var cd = imageObj.transform.GetChild(0).gameObject;
                var gcd = cd.transform.GetChild(0).gameObject;
                var image = gcd.GetComponent<Image>();
                image.sprite = t._sprites;
            }
        }

        public void ChangeItemImage(Sprite sprite)
        {
            _itemImage.sprite = sprite;
        }
    }
}
