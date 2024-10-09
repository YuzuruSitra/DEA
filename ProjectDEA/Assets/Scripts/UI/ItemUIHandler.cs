using Player;
using TMPro;
using Unity.VisualScripting;
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

        public void SetInventoryFrame(PlayerInventory.ItemPrefabSet[] itemSet)
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

        public void ChangeVisibleFrame(PlayerInventory.ItemPrefabSet[] itemSet)
        {
            for (var i = 0; i < itemSet.Length; i++)
            {
                _itemFrames[i].SetActive(itemSet[i]._count > 0);
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
