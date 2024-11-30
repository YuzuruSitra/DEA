using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputFunction
{
    public class InputSystemDeviceDetector : MonoBehaviour
    {
        public enum InputDeviceType
        {
            Keyboard,   // キーボード・マウス
            Xbox,       // Xboxコントローラー
            DualShock4, // DualShock4(PS4)
            DualSense,  // DualSense(PS5)
            Switch,     // SwitchのProコントローラー
        }
        // 直近に操作された入力デバイスタイプ
        public InputDeviceType CurrentDeviceType { get; private set; } = InputDeviceType.Keyboard;

        // 各デバイスのすべてのキーを１つにバインドしたInputAction（キー種別検知用）
        private InputAction _keyboardAnyKey = new InputAction(type: InputActionType.PassThrough, binding: "<Keyboard>/AnyKey", interactions: "Press");
        private InputAction _mouseAnyKey = new InputAction(type: InputActionType.PassThrough, binding: "<Mouse>/*", interactions: "Press");
        private InputAction _detectDualSenseAnyKey = new InputAction(type: InputActionType.PassThrough, binding: "<DualSenseGamepadHID>/*", interactions: "Press");
        private InputAction _switchProControllerAnyKey = new InputAction(type: InputActionType.PassThrough, binding: "<SwitchProControllerHID>/*", interactions: "Press");
        public event Action<InputDeviceType> OnInputDeviceTypeChanged;
        
        private void Start()
        {
            CheckSingleton();
        }

        private void OnDisable()
        {
            InputSystem.onDeviceChange -= OnDeviceChange;
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed)
            {
                Debug.Log($"デバイス変更: {device.displayName} 状態: {change}");
            }
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
            InputSystem.onDeviceChange += OnDeviceChange;
        }
    }
}