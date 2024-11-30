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
            OnChangeDevice?.Invoke(InputDeviceType.Keyboard);
        }
        
        private void ChangeGamePad(InputAction.CallbackContext context)
        {
            OnChangeDevice?.Invoke(InputDeviceType.GamePad);
        }
        
    }
}