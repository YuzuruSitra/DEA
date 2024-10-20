using System;
using System.Collections;
using Cinemachine;
using Item;
using Manager;
using UnityEngine;

namespace Gimmick
{
    public class ExitObelisk : MonoBehaviour, IInteractable
    {
        private DungeonLayerHandler _dungeonLayerHandler;
        private InventoryHandler _inventoryHandler;

        private const int NeededKeyCount = 1;
        private int _setKeyCount;
        [SerializeField] private CinemachineVirtualCameraBase _vCam;
        [SerializeField] private ParticleSystem _exitParticle;
        [SerializeField] private float _exitParticleDuration;
        private const int ParticleFactor = 7;
        private Coroutine _rateTimeCoroutine;
        
        [SerializeField] private GameObject[] _obeliskSides;
        [SerializeField] private ParticleSystem[] _obeliskSideParticles;
        [SerializeField] private float _sideEffectWaitTime;
        private WaitForSeconds _sideEffectWaitForSeconds;
        private const int Priority = 15;
        public event Action Destroyed;
        
        private void Start()
        {
            _dungeonLayerHandler = GameObject.FindWithTag("DungeonLayerHandler").GetComponent<DungeonLayerHandler>();
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            _sideEffectWaitForSeconds = new WaitForSeconds(_sideEffectWaitTime);
        }
        
        public void Interact()
        {
            if (_setKeyCount >= NeededKeyCount)
            {
                _vCam.Priority = Priority;
                StartCoroutine(ExitLayer());
                return;
            }

            if (_inventoryHandler.CurrentItemNum != (int)ItemKind.Key) return;
            if (_setKeyCount >= _obeliskSides.Length) return;
            _inventoryHandler.UseItem();
            StartCoroutine(SetKey());
            if (_rateTimeCoroutine != null) StopCoroutine(_rateTimeCoroutine);
            _rateTimeCoroutine = StartCoroutine(ChangeParticleRateTime(_setKeyCount * ParticleFactor));
        }

        private IEnumerator SetKey()
        {
            var currentKey = _setKeyCount;
            _setKeyCount++;
            _obeliskSideParticles[currentKey].Play();
            yield return _sideEffectWaitForSeconds;
            _obeliskSides[currentKey].SetActive(true);
        }

        private IEnumerator ExitLayer()
        {
            yield return new WaitForSeconds(3.0f);
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

            _rateTimeCoroutine = null;
        }
        
    }
}
