using System;
using System.Collections;
using Character.Player;
using Cinemachine;
using Manager;
using Manager.Audio;
using UI;
using UnityEngine;

namespace Gimmick
{
    public class StartObelisk : MonoBehaviour, IInteractable
    {
        [SerializeField] private DungeonLayerHandler _dungeonLayerHandler;
        
        [SerializeField] private CinemachineVirtualCameraBase _vCam;
        [SerializeField] private ParticleSystem _exitParticle;
        [SerializeField] private float _exitParticleDuration;
        private const int ParticleFactor = 7;
        
        [SerializeField] private GameObject[] _obeliskSides;
        [SerializeField] private ParticleSystem[] _obeliskSideParticles;
        [SerializeField] private float _sideEffectWaitTime;
        private WaitForSeconds _sideEffectWaitForSeconds;
        private const int HighPriority = 15;
        public event Action Destroyed;
        public bool IsInteractable => true;
        
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _setKeyAudio;
        [SerializeField] private float _camChangeWaitTime;
        [SerializeField] private float _logWaitTime;
        [SerializeField] private float _nextSceneTime;
        private WaitForSeconds _logWaitForSeconds;
        [SerializeField] private LogTextHandler _logTextHandler;
        [SerializeField] private PlayerClasHub _playerClasHub;
        [SerializeField] private string[] _explaneLogs;
        
        private void Awake()
        {
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
            _sideEffectWaitForSeconds = new WaitForSeconds(_sideEffectWaitTime);
            _logWaitForSeconds = new WaitForSeconds(_logWaitTime);
        }

        public void Interact()
        {
            _playerClasHub.SetPlayerFreedom(false);
            StartCoroutine(LaunchMission());
        }

        private IEnumerator LaunchMission()
        {
            // カメラ移動
            _vCam.Priority = HighPriority;
            
            // オベリスク起動
            _soundHandler.PlaySe(_setKeyAudio);
            foreach (var t in _obeliskSideParticles)
            {
                t.Play();
            }
            yield return _sideEffectWaitForSeconds;
            foreach (var t in _obeliskSides)
            {
                t.SetActive(true);
            }
            StartCoroutine(ChangeParticleRateTime(_obeliskSides.Length * ParticleFactor));
            
            // 会話
            foreach (var t in _explaneLogs)
            {
                _logTextHandler.AddLog(t);
                yield return _logWaitForSeconds;
            }
            // シーン遷移
            StartCoroutine(ExitLayer());
        }

        private IEnumerator ExitLayer()
        {
            yield return new WaitForSeconds(_nextSceneTime);
            _dungeonLayerHandler.NextDungeonLayer();
        }
        
        private IEnumerator ChangeParticleRateTime(float targetRate)
        {
            var emission = _exitParticle.emission;
            var startValue = emission.rateOverTime.constant;
            var elapsedTime = 0f;

            while (elapsedTime < _exitParticleDuration)
            {
                var newRate = Mathf.Lerp(startValue, targetRate, elapsedTime / _exitParticleDuration);
                var rate = emission.rateOverTime;
                rate.constant = newRate;
                emission.rateOverTime = rate;

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            var finalRate = emission.rateOverTime;
            finalRate.constant = targetRate;
            emission.rateOverTime = finalRate;
        }
        
    }
}
