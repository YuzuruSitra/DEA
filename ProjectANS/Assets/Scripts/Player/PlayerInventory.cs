using System;
using System.Collections.Generic;
using UnityEngine;
using Item;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        private Dictionary<ItemKind, int> items = new Dictionary<ItemKind, int>();

        private void Start()
        {
            foreach (ItemKind kind in Enum.GetValues(typeof(ItemKind)))
            {
                items[kind] = 0;  // 初期値として0を設定
            }
        }

        // アイテムをインベントリに追加する
        public void AddItem(ItemKind item)
        {
            if (items.ContainsKey(item))
                items[item]++;
            else
                items[item] = 1;
        }
    }
}