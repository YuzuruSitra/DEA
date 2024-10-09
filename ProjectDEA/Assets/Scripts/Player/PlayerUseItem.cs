using UnityEngine;

namespace Player
{
    public class PlayerUseItem : MonoBehaviour
    {
        [SerializeField] private PlayerInventory _playerInventory;
        [SerializeField] private float _checkRayLength;
        [SerializeField] private LayerMask _ignoreLayerMask;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) InsItem();
        }

        private void InsItem()
        {
            var item = _playerInventory.UseItem();
            if (item == null) return;

            bool frontBlocked = Physics.Raycast(transform.position, transform.forward, _checkRayLength, ~_ignoreLayerMask);
            bool rightBlocked = Physics.Raycast(transform.position, transform.right, _checkRayLength, ~_ignoreLayerMask);
            bool leftBlocked = Physics.Raycast(transform.position, -transform.right, _checkRayLength, ~_ignoreLayerMask);

            var basePos = transform.position;

            var spawnPosition = basePos + transform.forward;
            if (frontBlocked)
            {
                if (!rightBlocked)
                    spawnPosition = basePos + transform.right;
                else if (!leftBlocked)
                    spawnPosition = basePos - transform.right;
                else
                    return;
            }

            spawnPosition.x = Mathf.Round(spawnPosition.x * 2) / 2;
            spawnPosition.y = Mathf.Round(spawnPosition.x * 2) / 2;
            spawnPosition.z = Mathf.Round(spawnPosition.z * 2) / 2;

            Instantiate(item, spawnPosition, Quaternion.identity);
        }
    }
}
