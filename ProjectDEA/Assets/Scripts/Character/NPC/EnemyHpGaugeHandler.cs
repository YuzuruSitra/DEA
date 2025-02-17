using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Character.NPC
{
    public class EnemyHpGaugeHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _canvasObj;
        [SerializeField] private Slider _slider;
        [SerializeField] private float _waitingTime;
        [SerializeField] private float _disabledTime;
        private WaitForSeconds _disabledWait;
        private Coroutine _coroutine;
        private Camera _camera;
        private int _maxHp;

        private void Start()
        {
            _camera = Camera.main;
            _disabledWait = new WaitForSeconds(_disabledTime);
        }

        public void InitialSet(float maxHp, float currentHp)
        {
            _slider.maxValue = maxHp;
            _slider.value = currentHp;
            _canvasObj.SetActive(false);
        }

        private void LateUpdate() 
        {
            //　カメラと同じ向きに設定
            _canvasObj.transform.rotation = _camera.transform.rotation;
        }
        
        public void ChangeGauge(float newHealth)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            _canvasObj.SetActive(true);
            _coroutine = StartCoroutine(ChangeGageAnim(newHealth));
        }
    
        private IEnumerator ChangeGageAnim(float newHealth)
        {
            var elapsedTime = 0f;
            var startValue = _slider.value;
            newHealth = Mathf.Min(startValue, newHealth);

            while (elapsedTime < _waitingTime)
            {
                elapsedTime += Time.deltaTime;
                var t = Mathf.Clamp01(elapsedTime / _waitingTime);
                _slider.value = Mathf.Lerp(startValue, newHealth, t);
                yield return null;
            }
            _slider.value = newHealth;
            yield return _disabledWait;
            _canvasObj.SetActive(false);
            _coroutine = null;
        }
    }
}
