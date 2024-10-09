using UnityEngine;

namespace Player
{
    public class PlayerUseItem : MonoBehaviour
    {
        [SerializeField] private PlayerInventory _playerInventory;
        [SerializeField] private float _checkRayLength;
        [SerializeField] private LayerMask _ignoreLayerMask;

        private Vector3 _predictPosition;
        
        private void Update()
        {
            if (_playerInventory.CurrentPredict != null) MovePredict();

            if (Input.GetKeyDown(KeyCode.Space)) InsItem();
        }

        private void MovePredict()
        {
            _predictPosition = CalculateSpawnPosition();

            if (_predictPosition != Vector3.zero)
            {
                _playerInventory.CurrentPredict.transform.position = _predictPosition;
            }
        }

        private Vector3 CalculateSpawnPosition()
        {
            var frontBlocked = Physics.Raycast(transform.position, transform.forward, _checkRayLength, ~_ignoreLayerMask);
            var rightBlocked = Physics.Raycast(transform.position, transform.right, _checkRayLength, ~_ignoreLayerMask);
            var leftBlocked = Physics.Raycast(transform.position, -transform.right, _checkRayLength, ~_ignoreLayerMask);

            var basePos = transform.position;
            Vector3 spawnPosition;
            
            if (!frontBlocked)
            {
                spawnPosition = basePos + transform.forward;    
            }
            else if (!rightBlocked)
            {
                spawnPosition = basePos + transform.right;
            }
            else if (!leftBlocked)
            {
                spawnPosition = basePos - transform.right;
            }
            else
            {
                return Vector3.zero;
            }
            
            spawnPosition.x = Mathf.Round(spawnPosition.x * 2) / 2;
            spawnPosition.y = Mathf.Round(spawnPosition.y * 2) / 2;
            spawnPosition.z = Mathf.Round(spawnPosition.z * 2) / 2;

            return spawnPosition;
        }

        private void InsItem()
        {
            var item = _playerInventory.UseItem();
            if (item == null || _predictPosition == Vector3.zero) return;

            // 計算済みのPredictの位置にアイテムを生成
            Instantiate(item, _predictPosition, Quaternion.identity);
        }
    }
}
