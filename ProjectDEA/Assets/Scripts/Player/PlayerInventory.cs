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
            public Sprite _sprites;
            public int _count;
        }
        [SerializeField] private ItemPrefabSet[] _itemSets;
        [SerializeField] private ItemSpriteHandler _itemSpriteHandler;
        private int _currentItemNum;
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
            ChangeItemNum(1); // 次のアイテムに進む
        }

        public void DecreaseItemNum()
        {
            ChangeItemNum(-1); // 前のアイテムに戻る
        }

        private void ChangeItemNum(int step)
        {
            var newIndex = (_currentItemNum + step + _itemSets.Length) % _itemSets.Length;
            foreach (var t in _itemSets)
            {
                if (_itemSets[newIndex]._count > 0)
                {
                    _currentItemNum = newIndex;
                    _onItemNumChanged?.Invoke(_itemSets[_currentItemNum]._sprites);
                    return;
                }
                newIndex = (newIndex + step + _itemSets.Length) % _itemSets.Length;
            }
        }

        // アイテムをインベントリに追加する
        public void AddItem(ItemKind item)
        {
            Debug.Log("Add Inventory : " + item);
            for (var i = 0; i < _itemSets.Length; i++)
            {
                if (_itemSets[i]._kind != item) continue;
                _itemSets[i]._count++;
                return;
            }
        }
    }
}