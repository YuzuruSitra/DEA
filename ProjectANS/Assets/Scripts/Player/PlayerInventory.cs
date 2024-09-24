using System;
using UnityEngine;
using Item;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        [Serializable]
        public struct ItemPrefabSet
        {
            public ItemKind kind;
            public GameObject prefab;
            public int count;
        }
        [SerializeField]
        private ItemPrefabSet[] _itemSets;

        // アイテムをインベントリに追加する
        public void AddItem(ItemKind item)
        {
            for (int i = 0; i < _itemSets.Length; i++)
            {
                if (_itemSets[i].kind == item)
                {
                    _itemSets[i].count++;
                    return;
                }
            }
        }
    }
}