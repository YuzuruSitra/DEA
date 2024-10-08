using System;
using UnityEngine;
using Item;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        [Serializable]
        public struct ItemPrefabSet
        {
            public ItemKind _kind;
            public GameObject _prefab;
            public int _count;
        }
        [SerializeField]
        private ItemPrefabSet[] _itemSets;

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