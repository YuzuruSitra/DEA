using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LogTextDisable : MonoBehaviour
    {
        public TextMeshProUGUI TMPro { get; private set; }
        [SerializeField] private float _disableTime;
        private WaitForSeconds _disableWait;
        private Coroutine _disableCoroutine;

        private void Start()
        {
            TMPro = GetComponent<TextMeshProUGUI>();
            _disableWait = new WaitForSeconds(_disableTime);
        }

        public void ReceiveMessage(string message)
        {
            if (_disableCoroutine != null) StopCoroutine(_disableCoroutine);
            TMPro.enabled = true;
            TMPro.text = message;
            _disableCoroutine = StartCoroutine(OnDisableCoroutine());
        }

        private IEnumerator OnDisableCoroutine()
        {
            yield return _disableWait;
            TMPro.enabled = false;
            _disableCoroutine = null;
        }
    }
}
