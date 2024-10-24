using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LogTextDisable : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _tmPro;
        public TextMeshProUGUI TMPro => _tmPro;
        public string CurrentText { get; private set; }
        [SerializeField] private float _disableTime;
        private WaitForSeconds _disableWait;
        [SerializeField] private float _charDisplayInterval;
        private WaitForSeconds _charDisplayWait;
        private Coroutine _disableCoroutine;
        private Coroutine _displayCoroutine;
        [SerializeField] private bool _isAnimation;

        public void ReceiveMessage(string message, bool isDisable)
        {
            _disableWait ??= new WaitForSeconds(_disableTime);
            _charDisplayWait ??= new WaitForSeconds(_charDisplayInterval);
            CurrentText = message;
            // アニメーションのコルーチンが動いていたら停止
            if (_displayCoroutine != null) StopCoroutine(_displayCoroutine);

            // 非表示処理のコルーチンが動いていたら停止
            if (_disableCoroutine != null) StopCoroutine(_disableCoroutine);

            TMPro.enabled = true;

            // アニメーションが必要かどうかで処理を分岐
            if (_isAnimation)
            {
                _displayCoroutine = StartCoroutine(DisplayTextWithDelay(message, isDisable));
                return;
            }
            TMPro.text = message;
            if (isDisable) _disableCoroutine = StartCoroutine(OnDisableCoroutine());
        }

        // 1文字ずつ表示するコルーチン
        private IEnumerator DisplayTextWithDelay(string message, bool isDisable)
        {
            TMPro.text = "";
            foreach (var c in message)
            {
                TMPro.text += c;
                yield return _charDisplayWait;
            }
            if (isDisable) _disableCoroutine = StartCoroutine(OnDisableCoroutine());
            _displayCoroutine = null;
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
