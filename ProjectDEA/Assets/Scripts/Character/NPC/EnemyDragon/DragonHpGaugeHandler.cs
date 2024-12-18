using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Character.NPC.EnemyDragon
{
    public class DragonHpGaugeHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _canvasObj;
        [SerializeField] private Slider _slider;
        [SerializeField] private float _waitingTime;
        [SerializeField] private float _disabledTime;
        private WaitForSeconds _disabledWait;
        [SerializeField] private DragonController _dragonController;
        private Coroutine _coroutine;
        private Camera _camera;
        private int _maxHp;

        private void Start()
        {
            _camera = Camera.main;
            _maxHp = _dragonController.MaxHp;
            InitialSet();
            
            _dragonController.OnReviving += InitialSet;
            _dragonController.ReceiveNewHp += BeInjured;
            _disabledWait = new WaitForSeconds(_disabledTime);
        }

        private void InitialSet()
        {
            _slider.maxValue = _maxHp;
            _slider.value = _maxHp;
            _canvasObj.SetActive(false);
        }

        private void OnDestroy()
        {
            _dragonController.OnReviving -= InitialSet;
            _dragonController.ReceiveNewHp -= BeInjured;
        }

        private void LateUpdate() 
        {
            //　カメラと同じ向きに設定
            _canvasObj.transform.rotation = _camera.transform.rotation;
        }
        
        private void BeInjured(int newHp)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            _canvasObj.SetActive(true);
            _coroutine = StartCoroutine(ChangeGageAnim(newHp));
        }
    
        private IEnumerator ChangeGageAnim(float newHp)
        {
            var elapsedTime = 0f;
            var startValue = _slider.value;
            newHp = Mathf.Min(startValue, newHp);  // newHpが現在のスライダー値より高くならないように制限

            while (elapsedTime < _waitingTime)
            {
                elapsedTime += Time.deltaTime;
                var t = Mathf.Clamp01(elapsedTime / _waitingTime); // tを0〜1に制限
                _slider.value = Mathf.Lerp(startValue, newHp, t);
                yield return null;
            }
            _slider.value = newHp;
            yield return _disabledWait;
            _canvasObj.SetActive(false);
            _coroutine = null;
        }
    }
}
