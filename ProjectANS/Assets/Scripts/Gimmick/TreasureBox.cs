using UnityEngine;
using Item;
using Player;

namespace Gimmick
{
    public class TreasureBox : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private ItemKind[] _containeItem;
        private ItemKind _outItem;

        void Start()
        {
            var number = UnityEngine.Random.Range(0, _containeItem.Length);
            _outItem = _containeItem[number];
        }

        public void Interact()
        {
            var inventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
            inventory.AddItem(_outItem);
        }
    }
}
