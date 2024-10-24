using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LogTextDisable : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _tmPro;
        public TextMeshProUGUI TMPro => _tmPro;
        [SerializeField] private float _disableTime;
        private WaitForSeconds _disableWait;
        private Coroutine _disableCoroutine;

        public void ReceiveMessage(string message, bool isDisable)
        {
            _disableWait ??= new WaitForSeconds(_disableTime);
            if (_disableCoroutine != null) StopCoroutine(_disableCoroutine);
            TMPro.enabled = true;
            TMPro.text = message;
            if (isDisable) _disableCoroutine = StartCoroutine(OnDisableCoroutine());
        }

        private IEnumerator OnDisableCoroutine()
        {
            yield return _disableWait;
            TMPro.enabled = false;
            _disableCoroutine = null;
        }

        public void OnDisableTMPro()
        {
            TMPro.enabled = false;
        }
    }
}
