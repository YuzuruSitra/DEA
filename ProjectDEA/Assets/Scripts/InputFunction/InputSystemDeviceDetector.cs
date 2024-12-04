using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace InputFunction
{
    public class InputSystemDeviceDetector : MonoBehaviour
    {
        public enum InputDeviceType
        {
            Keyboard,
            GamePad
        }

        public InputDeviceType CurrentType { get; private set; }
        public event Action<InputDeviceType> OnChangeDevice;
        private InputActions _inputActions;
        private bool _addListen;
        
        private void Start()
        {
            CheckSingleton();
        }

        private void OnDestroy()
        {
            if (!_addListen) return;
            _inputActions.InputSeparate.InputKeyBoard.performed -= ChangeKeyBoard;
            _inputActions.InputSeparate.InputGamePad.performed -= ChangeGamePad;
            _inputActions.Disable();
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
            
            // シーンロードイベント登録
            SceneManager.sceneLoaded += OnSceneLoaded;
            _addListen = true;
        }

        private void ChangeKeyBoard(InputAction.CallbackContext context)
        {
            if (CurrentType == InputDeviceType.Keyboard) return;
            CurrentType = InputDeviceType.Keyboard;
            OnChangeDevice?.Invoke(CurrentType);
        }

        private void ChangeGamePad(InputAction.CallbackContext context)
        {
            if (CurrentType == InputDeviceType.GamePad) return;
            CurrentType = InputDeviceType.GamePad;
            OnChangeDevice?.Invoke(CurrentType);
        }

        // シーンロード時にデバイス状態を再通知
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            OnChangeDevice?.Invoke(CurrentType);
        }
    }
}
