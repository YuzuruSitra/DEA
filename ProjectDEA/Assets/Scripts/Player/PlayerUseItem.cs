using UnityEngine;

namespace Player
{
    public class PlayerUseItem : MonoBehaviour
    {
        [SerializeField] private PlayerInventory _playerInventory;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                InsItem();
            }
        }

        private void InsItem()
        {
            var item = _playerInventory.UseItem();
            if (item == null) return;
            Instantiate(item);
        }
    }
}
