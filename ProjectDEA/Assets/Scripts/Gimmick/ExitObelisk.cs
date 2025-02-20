using System;
using System.Collections;
using Cinemachine;
using Manager;
using Manager.Audio;
using Manager.MetaAI;
using Mission;
using UnityEngine;

namespace Gimmick
{
    public class ExitObelisk : MonoBehaviour, IInteractable, IGimmickID
    {
        private DungeonLayerHandler _dungeonLayerHandler;
        public MissionStateHandler MissionStateHandler { get; private set; }

        public const int NeededKeyCount = 4;
        private int _setKeyCount;
        [SerializeField] private int _oneMissionGetKeyCount;
        [SerializeField] private CinemachineVirtualCameraBase _vCam;
        [SerializeField] private ParticleSystem _exitParticle;
        [SerializeField] private float _exitParticleDuration;
        private const int ParticleFactor = 7;
        private Coroutine _rateTimeCoroutine;
        
        [SerializeField] private GameObject[] _obeliskSides;
        [SerializeField] private ParticleSystem[] _obeliskSideParticles;
        [SerializeField] private float _sideEffectWaitTime;
        private WaitForSeconds _sideEffectWaitForSeconds;
        private const int HighPriority = 15;
        private const int LowPriority = 0;
        public event Action Destroyed;
        public bool IsInteractable => !MissionStateHandler.DoingMission;
        private bool _isCompleted;
        
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _setKeyAudio;
        public GimmickID GimmickIdInfo { get; set; }
        public event Action<IGimmickID> Returned;
        [SerializeField] private float _rewardCamWaitTime;
        private WaitForSeconds _rewardCamWaitForSeconds;
        
        private void Start()
        {
            _dungeonLayerHandler = GameObject.FindWithTag("DungeonLayerHandler").GetComponent<DungeonLayerHandler>();
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
            _sideEffectWaitForSeconds = new WaitForSeconds(_sideEffectWaitTime);
            _rewardCamWaitForSeconds = new WaitForSeconds(_rewardCamWaitTime);
            var gameEventManager = GameObject.FindWithTag("GameEventManager").GetComponent<GameEventManager>();
            var roomGimmickGenerator = GameObject.FindWithTag("GimmickGenerator").GetComponent<RoomGimmickGenerator>();
            var inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            var metaAIHandler = GameObject.FindWithTag("MetaAI").GetComponent<MetaAIHandler>();
            MissionStateHandler = new MissionStateHandler(gameEventManager, roomGimmickGenerator, inventoryHandler, metaAIHandler);
            MissionStateHandler.OnMissionFinished += OnMissionCompleted;
        }

        private void OnDestroy()
        {
            MissionStateHandler.OnMissionFinished -= OnMissionCompleted;
            MissionStateHandler.Dispose();
        }

        public void Interact()
        {
            if (_isCompleted) return;
            if (_setKeyCount >= NeededKeyCount)
            {
                _isCompleted = true;
                _vCam.Priority = HighPriority;
                StartCoroutine(ExitLayer());
                return;
            }

            // ミッション開始
            if (MissionStateHandler.DoingMission) return;
            MissionStateHandler.StartMission();
            // 音とかセリフを追加予定
        }

        private void OnMissionCompleted()
        {
            if (_setKeyCount >= _obeliskSides.Length) return;
            // 音とかセリフを追加予定
            for (var i = 0; i < _oneMissionGetKeyCount; i++)
            {
                StartCoroutine(SetKey(_setKeyCount + i));
            }
            if (_rateTimeCoroutine != null) StopCoroutine(_rateTimeCoroutine);
            _rateTimeCoroutine = StartCoroutine(ChangeParticleRateTime((_setKeyCount + _oneMissionGetKeyCount) * ParticleFactor));
        }

        private IEnumerator SetKey(int keyIndex)
        {
            _vCam.Priority = HighPriority;
            yield return _rewardCamWaitForSeconds;
            _obeliskSideParticles[keyIndex].Play();
            _soundHandler.PlaySe(_setKeyAudio);
            yield return _sideEffectWaitForSeconds;
            _obeliskSides[keyIndex].SetActive(true);
            yield return _rewardCamWaitForSeconds;
            _vCam.Priority = LowPriority;
            _setKeyCount++;
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
