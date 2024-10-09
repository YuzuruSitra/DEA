using UnityEngine;

namespace Item
{
    public class Dynamite : MonoBehaviour, IItem
    {
        [SerializeField] private string _name;
        public string Name => _name;
        [SerializeField] private ItemKind _itemKind;
        public ItemKind ItemKind => _itemKind;
        [SerializeField] private string _description;
        public string Description => _description;
        [SerializeField] private bool _isConsumable;
        public bool IsConsumable => _isConsumable;
        [SerializeField] private float _detonationTime;
        private float _currentTime;
        private bool _isUsed;
        [SerializeField] private float _rayLength = 1.5f;

        private void Update()
        {
            if (_isUsed) return;

            _currentTime += Time.deltaTime;
            if (!(_currentTime > _detonationTime)) return;
            UseEffect();
            _isUsed = true;
        }

        public void UseEffect()
        {
            var directions = SetDirections();
            
            var adPos = AdjustedPosition();
            PerformRaycastInDirections(directions, adPos);
            PerformRaycastInDirections(directions, adPos + Vector3.up);
            Destroy(gameObject);
        }

        private void PerformRaycastInDirections(Vector3[] directions, Vector3 origin)
        {
            foreach (var direction in directions)
            {
                var ray = new Ray(origin, direction);
                var hits = Physics.RaycastAll(ray, _rayLength);
                
                foreach (var hit in hits)
                {
                    if (hit.collider.CompareTag("StageCube"))
                    {
                        Destroy(hit.collider.gameObject);
                    }
                }
            }
        }
        
        private static Vector3[] SetDirections()
        {
            var directions = new Vector3[9];
            directions[0] = Vector3.forward;     // +Z 方向
            directions[1] = Vector3.back;        // -Z 方向
            directions[2] = Vector3.right;       // +X 方向
            directions[3] = Vector3.left;        // -X 方向
            directions[4] = Vector3.up;
            directions[5] = (Vector3.forward + Vector3.right).normalized;  // +Z +X 方向
            directions[6] = (Vector3.forward + Vector3.left).normalized;   // +Z -X 方向
            directions[7] = (Vector3.back + Vector3.right).normalized;     // -Z +X 方向
            directions[8] = (Vector3.back + Vector3.left).normalized;      // -Z -X 方向
            return directions;
        }

        private Vector3 AdjustedPosition()
        {
            var adjustedPosition = Vector3.zero;
            adjustedPosition.x = transform.position.x;
            adjustedPosition.y = Mathf.Round(transform.position.y * 2) / 2;
            adjustedPosition.z = transform.position.z;
            return adjustedPosition;
        }
    }
}
