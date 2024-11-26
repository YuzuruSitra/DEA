using System;
using System.Collections;
using Cinemachine;
using Item;
using Manager;
using Manager.Audio;
using UnityEngine;

namespace Gimmick
{
    public class ExitObelisk : MonoBehaviour, IInteractable, IGimmickID
    {
        private DungeonLayerHandler _dungeonLayerHandler;
        private InventoryHandler _inventoryHandler;

        public const int NeededKeyCount = 4;
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
        public bool IsInteractable { get; private set; }
        private bool _isCompleted;
        
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _setKeyAudio;
        public int InRoomID { get; set; }
        
        private void Start()
        {
            _dungeonLayerHandler = GameObject.FindWithTag("DungeonLayerHandler").GetComponent<DungeonLayerHandler>();
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
            _sideEffectWaitForSeconds = new WaitForSeconds(_sideEffectWaitTime);
            _inventoryHandler.OnItemNumChanged += CheckItemInteractable;
        }

        private void OnDestroy()
        { 
            if (!_isCompleted) _inventoryHandler.OnItemNumChanged -= CheckItemInteractable;
        }

        public void Interact()
        {
            if (_isCompleted) return;
            if (_setKeyCount >= NeededKeyCount)
            {
                _isCompleted = true;
                IsInteractable = false;
                _inventoryHandler.OnItemNumChanged -= CheckItemInteractable;
                _vCam.Priority = Priority;
                StartCoroutine(ExitLayer());
                return;
            }

            if (_inventoryHandler.CurrentItemNum != (int)ItemKind.Key) return;
            if (_setKeyCount >= _obeliskSides.Length) return;
            StartCoroutine(SetKey());
            _inventoryHandler.UseItem();
            if (_rateTimeCoroutine != null) StopCoroutine(_rateTimeCoroutine);
            _rateTimeCoroutine = StartCoroutine(ChangeParticleRateTime(_setKeyCount * ParticleFactor));
        }
        
        private IEnumerator SetKey()
        {
            var currentKey = _setKeyCount;
            _setKeyCount++;
            _obeliskSideParticles[currentKey].Play();
            _soundHandler.PlaySe(_setKeyAudio);
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

        private void CheckItemInteractable()
        {
            if (_setKeyCount >= NeededKeyCount) return;
            IsInteractable = _inventoryHandler.CurrentItemNum == (int)ItemKind.Key;
        }
        
    }
}
