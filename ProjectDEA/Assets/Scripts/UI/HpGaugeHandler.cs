using System.Collections;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HpGaugeHandler : MonoBehaviour
    {
        private Slider _slider;
        [SerializeField] private float _waitingTime;
        private Coroutine _coroutine;
        private PlayerStatusHandler _playerStatusHandler;
        
        private void Start()
        {
            _playerStatusHandler = GameObject.FindWithTag("PlayerStatusHandler").GetComponent<PlayerStatusHandler>();
            
            var maxHp = _playerStatusHandler.MaxHp;
            var currentHp = _playerStatusHandler.PlayerCurrentHp;
            
            _slider = gameObject.GetComponent<Slider>();
            _slider.maxValue = maxHp;
            _slider.value = currentHp;
            
            _playerStatusHandler.OnChangeHp += BeInjured;
        }

        private void OnDestroy()
        {
            _playerStatusHandler.OnChangeHp -= BeInjured;
        }
        
        private void BeInjured(int newHp)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            if (gameObject.activeInHierarchy) _coroutine = StartCoroutine(ChangeGageAnim(newHp));
        }
    
        private IEnumerator ChangeGageAnim(float newHp)
        {
            var elapsedTime = 0f;
            while (elapsedTime < _waitingTime)
            {
                elapsedTime += Time.deltaTime;
                var currentValue = Mathf.Lerp(_slider.value, newHp, elapsedTime / _waitingTime);
                _slider.value = currentValue;
                yield return null;
            }
            _slider.value = newHp;
        }
    }
}