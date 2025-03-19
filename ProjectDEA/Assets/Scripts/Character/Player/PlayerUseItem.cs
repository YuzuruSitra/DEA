using Item;
using Manager;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

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
        private InputActions _inputActions;
        public bool CanUseCandle { get; set; }
        private readonly string[] _candleLogTemplate =
        {
            "他のキャンドルと少し近いな...",
            "A little close to the other candles..."
        };
        [SerializeField] private PanelSwitcher _panelSwitcher;
        
        private void Start()
        {
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            _logTextHandler = GameObject.FindWithTag("LogTextHandler").GetComponent<LogTextHandler>();
            _inventoryHandler.OnItemNumChanged += ResetState;

            // InputActions をインスタンス化
            _inputActions = new InputActions();
            _inputActions.Player.UseItem.performed += OnUseItem;
            _inputActions.Player.PutCancel.performed += OnPutCancel;
            _inputActions.Enable();
            CanUseCandle = true;
        }

        private void OnDestroy()
        {
            _inventoryHandler.OnItemNumChanged -= ResetState;
            _inputActions.Player.UseItem.performed -= OnUseItem;
            _inputActions.Player.PutCancel.performed -= OnPutCancel;
            _inputActions.Disable();
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
            if (!target._isPut) return;
            if (_insState == InsState.Predict) MovePrediction();
        }

        private void OnUseItem(InputAction.CallbackContext context)
        {
            if (!_isCanUseItem) return;
            if (_inventoryHandler.CurrentItemNum == InventoryHandler.ErrorValue) return;

            var target = _inventoryHandler.TargetItem;

            if (target._kind == ItemKind.SignCandle && !CanUseCandle)
            {
                var language = _logTextHandler.LanguageHandler.CurrentLanguage;
                _logTextHandler.AddLog(_candleLogTemplate[(int)language]);
                ResetState();
                return;
            }
            
            if (target._isPut)
            {
                switch (_insState)
                {
                    case InsState.None:
                        _inventoryHandler.ChangePredictActive(target._currentPredict, true);
                        _insState = InsState.Predict;
                        break;
                    case InsState.Predict:
                        PlaceItem();
                        ResetState();
                        break;
                }
            }

            if (target._isUse) DoneUseItem();
        }

        private void OnPutCancel(InputAction.CallbackContext context)
        {
            ResetState();
        }

        private void MovePrediction()
        {
            if (_inventoryHandler.CurrentItemNum == InventoryHandler.ErrorValue) return;
            _predictedPosition = CalculateSpawnPosition();
            _predictedRotation = transform.rotation * Quaternion.Euler(0, 180, 0);
            var currentPrefab = _inventoryHandler.TargetItem._currentPredict;
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
            value.y = Mathf.Round(position.y) + 0.25f;
            value.z = Mathf.Floor(position.z) + 0.5f;
            return value;
        }

        private void PlaceItem()
        {
            var targetItem = _inventoryHandler.TargetItem;
            var item = targetItem._prefab;
            if (item == null || _predictedPosition == Vector3.zero) return;
            Instantiate(item, _predictedPosition, _predictedRotation);
            var language = _logTextHandler.LanguageHandler.CurrentLanguage;
            SendLogText(targetItem._effectedLogText[(int)language]);
            _inventoryHandler.UseItem();
        }

        private void ResetState()
        {
            if (_inventoryHandler.CurrentItemNum == InventoryHandler.ErrorValue) return;
            _insState = InsState.None;
            _inventoryHandler.ChangePredictActive(_inventoryHandler.TargetItem._currentPredict, false);
        }

        private void DoneUseItem()
        {
            var target = _inventoryHandler.TargetItem;
            _inventoryHandler.UseItem();
            var language = _logTextHandler.LanguageHandler.CurrentLanguage;
            switch (target._kind)
            {
                case ItemKind.PowerPotion:
                    _useItemEffects.PlayerPowerUpper();
                    SendLogText(target._effectedLogText[(int)language]);
                    break;
                case ItemKind.PowerApple:
                    _useItemEffects.PlayerRecovery();
                    SendLogText(target._effectedLogText[(int)language]);
                    break;
                case ItemKind.RaggedMemoirs:
                    _panelSwitcher.ChangeMemoirsPanel(true);
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
