using Item;
using Manager;
using UI;
using UnityEngine;

namespace Character.Player
{
    public class PlayerUseItem : MonoBehaviour
    {
        private bool _isCanUseItem = true;
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
        [SerializeField] private UseItemEffects _useItemEffects;
        private LogTextHandler _logTextHandler;
        
        private void Start()
        {
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            _logTextHandler = GameObject.FindWithTag("LogTextHandler").GetComponent<LogTextHandler>();
            _inventoryHandler.OnItemNumChanged += ResetState;
        }

        private void OnDestroy()
        {
            _inventoryHandler.OnItemNumChanged -= ResetState;
        }

        private void Update()
        {
            if (!_isCanUseItem) return;
            HandleItemUsage();
        }

        private void HandleItemUsage()
        {
            if (_inventoryHandler.CurrentItemNum == InventoryHandler.ErrorValue) return;
            var target = _inventoryHandler.TargetItem;
            if (target._isPut)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    switch (_insState)
                    {
                        case InsState.None:
                            _inventoryHandler.ChangePredictActive(target._currentPrefab, true);
                            _insState = InsState.Predict;
                            break;
                        case InsState.Predict:
                            PlaceItem();
                            ResetState();
                            SendLogText(target._effectedLogText);
                            break;
                    }
                }

                if (_insState == InsState.Predict) MovePrediction();

                // Chancel.
                if (Input.GetMouseButtonDown(1)) ResetState();
            }

            if (target._isUse && Input.GetKeyDown(KeyCode.Space)) DoneUseItem();
        }
        
        // PutItem
        private void MovePrediction()
        {
            _predictedPosition = CalculateSpawnPosition();
            _predictedRotation = transform.rotation * Quaternion.Euler(0, 180, 0);
            var currentPrefab = _inventoryHandler.TargetItem._currentPrefab;
            if (currentPrefab == null) return;
            currentPrefab.transform.position = _predictedPosition;
            currentPrefab.transform.rotation = _predictedRotation;
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
            if (_inventoryHandler.CurrentItemNum == InventoryHandler.ErrorValue) return;
            _insState = InsState.None;
            _inventoryHandler.ChangePredictActive(_inventoryHandler.TargetItem._currentPrefab, false);
        }
        
        // UseItem
        private void DoneUseItem()
        {
            var target = _inventoryHandler.TargetItem;
            _inventoryHandler.UseItem();
            switch (target._kind)
            {
                case ItemKind.PowerPotion:
                    _useItemEffects.PlayerPowerUpper();
                    SendLogText(target._effectedLogText);
                    break;
            }
        }

        private void SendLogText(string message)
        {
            if (message == "") return;
            _logTextHandler.AddLog(message);
        }
        
        public void SetCanUseItemState(bool active)
        {
            _isCanUseItem = active;
        }
    }
}