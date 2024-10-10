using UnityEngine;
using Item;
using Player;

namespace Gimmick
{
    public class TreasureBox : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private ItemKind[] _containItem;
        private ItemKind _outItem;
        private bool _isOpen;

        private void Start()
        {
            var number = Random.Range(0, _containItem.Length);
            _outItem = _containItem[number];
        }

        public void Interact()
        {
            if (_isOpen) return;
            var inventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
            inventory.AddItem(_outItem);
            _isOpen = true;
        }
    }
}
