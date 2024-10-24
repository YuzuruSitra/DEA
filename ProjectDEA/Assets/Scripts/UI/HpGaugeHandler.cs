using System.Collections;
using Character.Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HpGaugeHandler : MonoBehaviour
    {
        private Slider _slider;
        [SerializeField] private float _waitingTime;
        private PlayerHpHandler _playerHpHandler;
        private Coroutine _coroutine;
        
        private void Start()
        {
            var playerHub = GameObject.FindWithTag("Player").GetComponent<PlayerClasHub>();
            _playerHpHandler = playerHub.PlayerHpHandler;
            var maxHp = _playerHpHandler.MaxHp;
            var currentHp = _playerHpHandler.CurrentHp;
            
            _slider = gameObject.GetComponent<Slider>();
            _slider.maxValue = maxHp;
            _slider.value = currentHp;
            
            _playerHpHandler.OnChangeHp += BeInjured;
        }

        private void OnDestroy()
        {
            _playerHpHandler.OnChangeHp -= BeInjured;
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