using InputFunction;
using UnityEngine;

namespace UI
{
    public class DeviceUIChanger : MonoBehaviour
    {
        [SerializeField] private GameObject[] _keyboardUIs;
        [SerializeField] private GameObject[] _gamePadUIs;
        private InputSystemDeviceDetector _deviceDetector;
        
        private void Awake()
        {
            _deviceDetector = GameObject.FindWithTag("InputSystemDeviceDetector").GetComponent<InputSystemDeviceDetector>();
            _deviceDetector.OnChangeDevice += ChangeUI;
            ChangeUI(_deviceDetector.CurrentType);
        }

        private void OnDestroy()
        {
            _deviceDetector.OnChangeDevice -= ChangeUI;
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
