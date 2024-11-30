using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputFunction
{
    public class InputSystemDeviceDetector : MonoBehaviour
    {
        public enum InputDeviceType
        {
            Keyboard,
            GamePad
        }

        private InputDeviceType _currentType;
        public event Action<InputDeviceType> OnChangeDevice;
        private InputActions _inputActions;
        
        private void Start()
        {
            CheckSingleton();
        }

        private void OnDestroy()
        {
            _inputActions.InputSeparate.InputKeyBoard.performed -= ChangeKeyBoard;
            _inputActions.InputSeparate.InputGamePad.performed -= ChangeGamePad;
            _inputActions.Disable();
        }

        private void CheckSingleton()
        {
            var target = GameObject.FindGameObjectWithTag(gameObject.tag);
            var checkResult = target != null && target != gameObject;
            
            if (checkResult)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            // リスナー登録
            _inputActions = new InputActions();
            _inputActions.InputSeparate.InputKeyBoard.performed += ChangeKeyBoard;
            _inputActions.InputSeparate.InputGamePad.performed += ChangeGamePad;
            _inputActions.Enable();
        }

        private void ChangeKeyBoard(InputAction.CallbackContext context)
        {
            if (_currentType == InputDeviceType.Keyboard) return;
            _currentType = InputDeviceType.Keyboard;
            OnChangeDevice?.Invoke(_currentType);
        }
        
        private void ChangeGamePad(InputAction.CallbackContext context)
        {
            if (_currentType == InputDeviceType.GamePad) return;
            _currentType = InputDeviceType.GamePad;
            OnChangeDevice?.Invoke(_currentType);
        }
        
    }
}