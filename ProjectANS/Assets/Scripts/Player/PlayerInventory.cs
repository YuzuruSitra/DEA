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
            public ItemKind Kind;
            public GameObject Prefab;
            public int Count;
        }
        [SerializeField]
        private ItemPrefabSet[] _itemSets;

        // アイテムをインベントリに追加する
        public void AddItem(ItemKind item)
        {
            Debug.Log("Add Inventory : " + item);
            for (int i = 0; i < _itemSets.Length; i++)
            {
                if (_itemSets[i].Kind == item)
                {
                    _itemSets[i].Count++;
                    return;
                }
            }
        }
    }
}