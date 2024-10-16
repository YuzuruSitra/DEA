using Manager;
using UnityEngine;

namespace Player
{
    public class PlayerUseItem : MonoBehaviour
    {
        private enum InsState
        {
            None,
            Predict,
        }
        private InsState _insState = InsState.None; 
        private InventoryHandler _inventoryHandler;
        [SerializeField] private float _checkRayLength;
        [SerializeField] private LayerMask _ignoreLayerMask;

        private Vector3 _predictedPosition;
        private Quaternion _predictedRotation;

        private void Start()
        {
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            _inventoryHandler.OnItemNumChanged += ResetState;
        }

        private void OnDestroy()
        {
            _inventoryHandler.OnItemNumChanged -= ResetState;
        }

        private void Update()
        {
            HandleItemUsage();
        }

        private void HandleItemUsage()
        {
            if (!_inventoryHandler.CurrentIsUse) return;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                switch (_insState)
                {
                    case InsState.None:
                        _inventoryHandler.ChangePredictActive(_inventoryHandler.CurrentPredict, true);
                        _insState = InsState.Predict;
                        break;
                    case InsState.Predict:
                        PlaceItem();
                        ResetState();
                        break;
                }
            }
            
            if (_insState == InsState.Predict) MovePrediction();
            
            // Chancel.
            if (Input.GetMouseButtonDown(1)) ResetState();
        }

        private void MovePrediction()
        {
            _predictedPosition = CalculateSpawnPosition();
            _predictedRotation = transform.rotation * Quaternion.Euler(0, 180, 0);
            if (_inventoryHandler.CurrentPredict == null) return;
            _inventoryHandler.CurrentPredict.transform.position = _predictedPosition;
            _inventoryHandler.CurrentPredict.transform.rotation = _predictedRotation;
        }

        private Vector3 CalculateSpawnPosition()
        {
            if (!IsBlocked(transform.forward)) return AdjustToGrid(transform.position + transform.forward);
            if (!IsBlocked(transform.right)) return AdjustToGrid(transform.position + transform.right);
            return !IsBlocked(-transform.right) ? AdjustToGrid(transform.position - transform.right) : Vector3.zero;
        }

        private bool IsBlocked(Vector3 direction)
        {
            return Physics.Raycast(transform.position, direction, _checkRayLength, ~_ignoreLayerMask);
        }

        private static Vector3 AdjustToGrid(Vector3 position)
        {
            var value = Vector3.zero;
            value.x = Mathf.Floor(position.x) + 0.5f;
            value.y = Mathf.Round(position.y);
            value.z = Mathf.Floor(position.z) + 0.5f;
            return value;
        }

        private void PlaceItem()
        {
            var item = _inventoryHandler.UseItem();
            if (item == null || _predictedPosition == Vector3.zero) return;
            Instantiate(item, _predictedPosition, _predictedRotation);
        }

        private void ResetState()
        {
            _insState = InsState.None;
            _inventoryHandler.ChangePredictActive(_inventoryHandler.CurrentPredict, false);
        }
    }
}