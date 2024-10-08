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
            _currentItemNum++;
            if (_itemSets.Length <= _currentItemNum) _currentItemNum = 0;
            _onItemNumChanged?.Invoke(_itemSets[_currentItemNum]._sprites);
        }
        
        public void DecreaseItemNum()
        {
            _currentItemNum--;
            if (_currentItemNum < 0) _currentItemNum = _itemSets.Length - 1;
            _onItemNumChanged?.Invoke(_itemSets[_currentItemNum]._sprites);
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