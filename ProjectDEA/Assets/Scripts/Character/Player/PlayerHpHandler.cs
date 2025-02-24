using System;
using System.Collections;
using Manager;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Character.Player
{
    public class PlayerHpHandler : MonoBehaviour
    {
        [SerializeField] private PlayerAnimationCnt _playerAnimationCnt;
        public Action OnDie;
        public bool IsDie { get; private set; }
        private DungeonLayerHandler _dungeonLayerHandler;
        private PlayerStatusHandler _playerStatusHandler;
        public bool IsAddDamage { get; private set; } = true;
        [SerializeField] private PostProcessVolume _postProcessVolume;
        private Vignette _vignette;
        [SerializeField] private Color[] _vignetteColors;
        [SerializeField] private float _vignetteDuratrion;
        private Coroutine _vignetteCoroutine;
        
        private void Start()
        {
            _dungeonLayerHandler = GameObject.FindWithTag("DungeonLayerHandler").GetComponent<DungeonLayerHandler>();
            _playerStatusHandler = GameObject.FindWithTag("PlayerStatusHandler").GetComponent<PlayerStatusHandler>();
            _postProcessVolume.profile.TryGetSettings(out _vignette);
            OnDie += _playerAnimationCnt.SetIsDie;
            OnDie += _dungeonLayerHandler.PlayerDeathNext;
        }
        
        private void OnDestroy()
        {
            OnDie -= _playerAnimationCnt.SetIsDie;
            OnDie -= _dungeonLayerHandler.PlayerDeathNext;
        }

        public void ReceiveDamage(int damage)
        {
            if (IsDie) return;
            var newHp = Math.Max(_playerStatusHandler.PlayerCurrentHp - damage, 0);
            newHp = Math.Min(newHp, _playerStatusHandler.MaxHp);
            _playerStatusHandler.SetPlayerCurrentHp(newHp);
            
            if (_vignette != null && damage > 0)
            {
                if (_vignetteCoroutine != null)
                {
                    StopCoroutine(_vignetteCoroutine);
                }
                _vignetteCoroutine = StartCoroutine(AnimateVignette());
            }
            
            if (newHp > 0) return;
            IsDie = true;
            OnDie?.Invoke();
        }
        
        private IEnumerator AnimateVignette()
        {
            var elapsedTime = 0f;
            while (elapsedTime < _vignetteDuratrion / 2)
            {
                _vignette.color.value = Color.Lerp(_vignetteColors[0], _vignetteColors[1], elapsedTime / (_vignetteDuratrion / 2));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            elapsedTime = 0f;
            while (elapsedTime < _vignetteDuratrion / 2)
            {
                _vignette.color.value = Color.Lerp(_vignetteColors[1], _vignetteColors[0], elapsedTime / (_vignetteDuratrion / 2));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            _vignetteCoroutine = null;
        }
        
        public void ChangeIsAddDamage(bool state)
        {
            IsAddDamage = state;
        }
        
    }
}
