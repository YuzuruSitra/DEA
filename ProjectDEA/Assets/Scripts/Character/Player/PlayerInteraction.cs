using Gimmick;
using InputFunction;
using UnityEngine;

namespace Character.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        private bool _isInteractable = true;
        private IInteractable _currentInteractable;
        [SerializeField] private GameObject _indicationUI;
        private InputActions _inputActions;
        [SerializeField] private GameObject[] _keyboardUIs;
        [SerializeField] private GameObject[] _gamePadUIs;
        private InputSystemDeviceDetector _deviceDetector;

        private void Start()
        {
            // Interactアクションのイベントリスナーを登録
            _inputActions = new InputActions();
            _inputActions.Player.Interact.performed += OnInteractPerformed;
            _inputActions.Enable();
            
            _deviceDetector = GameObject.FindWithTag("InputSystemDeviceDetector").GetComponent<InputSystemDeviceDetector>();
            _deviceDetector.OnChangeDevice += ChangeUI;
        }

        private void OnDestroy()
        {
            // Interactアクションのイベントリスナーを解除
            _inputActions.Player.Interact.performed -= OnInteractPerformed;
            _inputActions.Disable();
            _deviceDetector.OnChangeDevice -= ChangeUI;
        }

        private void Update()
        {
            if (!_isInteractable) return;
            if (_currentInteractable == null) return;
            _indicationUI.SetActive(_currentInteractable.IsInteractable);
        }

        private void OnInteractPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            // インタラクト可能かチェックし、インタラクトを実行
            if (_currentInteractable != null && _currentInteractable.IsInteractable)
            {
                _currentInteractable.Interact();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var interactable = other.GetComponent<IInteractable>();
            if (interactable == null) return;
            _currentInteractable = interactable;
            _currentInteractable.Destroyed += ResetCurrentTarget;
        }

        private void OnTriggerExit(Collider other)
        {
            var interactable = other.GetComponent<IInteractable>();
            if (interactable == null || _currentInteractable != interactable) return;
            ResetCurrentTarget();
        }

        private void ResetCurrentTarget()
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.Destroyed -= ResetCurrentTarget;
                _currentInteractable = null;
            }
            _indicationUI.SetActive(false);
        }

        public void SetInteractableState(bool active)
        {
            _isInteractable = active;
        }

        private void ChangeUI(InputSystemDeviceDetector.InputDeviceType type)
        {
            switch (type)
            {
                case InputSystemDeviceDetector.InputDeviceType.Keyboard:
                    foreach (var t in _gamePadUIs)
                    {
                        t.SetActive(false);
                    }
                    foreach (var t in _keyboardUIs)
                    {
                        t.SetActive(true);
                    }
                    break;
                case InputSystemDeviceDetector.InputDeviceType.GamePad:
                    foreach (var t in _keyboardUIs)
                    {
                        t.SetActive(false);
                    }
                    foreach (var t in _gamePadUIs)
                    {
                        t.SetActive(true);
                    }
                    break;
            }
        }

    }
}
