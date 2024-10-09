using System;
using UnityEngine;
using Item;
using UI;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        [Serializable]
        public struct ItemPrefabSet
        {
            public ItemKind _kind;
            public GameObject _prefab;
            public Sprite _sprite;
            public int _count;
        }
        [SerializeField] private ItemPrefabSet[] _itemSets;
        [SerializeField] private ItemSpriteHandler _itemSpriteHandler;
        private const int ErrorValue = -1;
        private int _currentItemNum = ErrorValue;
        private Action<Sprite> _onItemNumChanged;
        
        private void Start()
        {
            _itemSpriteHandler.SetInventoryFrame(_itemSets);
            _onItemNumChanged += _itemSpriteHandler.ChangeItemImage;
        }

        private void OnDestroy()
        {
            _onItemNumChanged -= _itemSpriteHandler.ChangeItemImage;
        }

        public void IncreaseItemNum()
        {
            int startIndex = Math.Max(_currentItemNum, 0);

            for (int i = 1; i <= _itemSets.Length; i++)
            {
                int newIndex = (startIndex + i) % _itemSets.Length;

                if (_itemSets[newIndex]._count > 0)
                {
                    ChangeItemNum(newIndex);
                    return;
                }
            }

            ChangeItemNum(ErrorValue);
        }

        public void DecreaseItemNum()
        {
            int startIndex = _currentItemNum == ErrorValue ? _itemSets.Length - 1 : _currentItemNum;

            for (int i = 1; i <= _itemSets.Length; i++)
            {
                int newIndex = (startIndex - i + _itemSets.Length) % _itemSets.Length;

                if (_itemSets[newIndex]._count > 0)
                {
                    ChangeItemNum(newIndex);
                    return;
                }
            }

            ChangeItemNum(ErrorValue);
        }

        private void ChangeItemNum(int value)
        {
            _currentItemNum = value;
            var sprite = _currentItemNum != ErrorValue ? _itemSets[value]._sprite : null;
            _onItemNumChanged?.Invoke(sprite);
        }

        // アイテムをインベントリに追加する
        public void AddItem(ItemKind item)
        {
            Debug.Log("Add Inventory : " + item);
            for (var i = 0; i < _itemSets.Length; i++)
            {
                if (_itemSets[i]._kind != item) continue;
                _itemSets[i]._count++;
                if (_currentItemNum == ErrorValue) ChangeItemNum(i);
                break;
            }
        }

        public GameObject UseItem()
        {
            if (_currentItemNum == ErrorValue) return null;
            var itemSet = _itemSets[_currentItemNum];
            itemSet._count--;
            if (itemSet._count <= 0) ChangeItemNum(ErrorValue);
            return itemSet._prefab;
        }
    }
}