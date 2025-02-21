using System;
using System.Collections;
using Character.Player;
using Cinemachine;
using Manager;
using Manager.Audio;
using Manager.MetaAI;
using Mission;
using UI;
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
        [SerializeField] private float _camChangeWaitTime;
        private WaitForSeconds _camChangeForSeconds;
        [SerializeField] private float _logWaitTime;
        private WaitForSeconds _logWaitForSeconds;
        private LogTextHandler _logTextHandler;
        private PlayerClasHub _playerClasHub;
        [SerializeField] private CinemachineVirtualCameraBase _targetCam;
        
        private void Awake()
        {
            _dungeonLayerHandler = GameObject.FindWithTag("DungeonLayerHandler").GetComponent<DungeonLayerHandler>();
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
            _sideEffectWaitForSeconds = new WaitForSeconds(_sideEffectWaitTime);
            _camChangeForSeconds = new WaitForSeconds(_camChangeWaitTime);
            var gameEventManager = GameObject.FindWithTag("GameEventManager").GetComponent<GameEventManager>();
            var roomGimmickGenerator = GameObject.FindWithTag("GimmickGenerator").GetComponent<RoomGimmickGenerator>();
            var inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            var metaAIHandler = GameObject.FindWithTag("MetaAI").GetComponent<MetaAIHandler>();
            MissionStateHandler = new MissionStateHandler(gameEventManager, roomGimmickGenerator, inventoryHandler, metaAIHandler);
            MissionStateHandler.OnMissionFinished += OnMissionCompleted;
            _logWaitForSeconds = new WaitForSeconds(_logWaitTime);
            _logTextHandler = GameObject.FindWithTag("LogTextHandler").GetComponent<LogTextHandler>();
            _playerClasHub = GameObject.FindWithTag("Player").GetComponent<PlayerClasHub>();
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
                _playerClasHub.SetPlayerFreedom(false);
                StartCoroutine(ExitLayer());
                return;
            }

            // ミッション開始
            if (MissionStateHandler.DoingMission) return;
            StartCoroutine(LaunchMission());
        }

        private IEnumerator LaunchMission()
        {
            MissionStateHandler.StartMission();
            _playerClasHub.SetPlayerFreedom(false);
            _vCam.Priority = HighPriority;
            _soundHandler.PlaySe(_setKeyAudio);
            var logs = MissionStateHandler.CurrentMission.MissionLaunchLog;
            foreach (var t in logs)
            {
                _logTextHandler.AddLog(t);
                yield return _logWaitForSeconds;
            }

            var target = MissionStateHandler.CurrentMission.StandOutTarget;
            if (target != null)
            {
                _targetCam.Follow = target.transform;
                _targetCam.LookAt = target.transform;
                _targetCam.Priority = HighPriority;
                _vCam.Priority = LowPriority;
                yield return _camChangeForSeconds;
                _targetCam.Priority = LowPriority;
                _vCam.Priority = HighPriority;
            }

            yield return _camChangeForSeconds;
            _vCam.Priority = LowPriority;
            _playerClasHub.SetPlayerFreedom(true);
        }
        
        private void OnMissionCompleted()
        {
            if (_setKeyCount >= _obeliskSides.Length) return;
    
            StartCoroutine(CompletedSetKey(_oneMissionGetKeyCount));
    
            if (_rateTimeCoroutine != null) StopCoroutine(_rateTimeCoroutine);
            _rateTimeCoroutine = StartCoroutine(ChangeParticleRateTime((_setKeyCount + _oneMissionGetKeyCount) * ParticleFactor));
        }

        private IEnumerator CompletedSetKey(int getKeyCount)
        {
            _vCam.Priority = HighPriority;
            _playerClasHub.SetPlayerFreedom(false);
            yield return _camChangeForSeconds;

            var logs = MissionStateHandler.CurrentMission.MissionFinishLog;
            foreach (var t in logs)
            {
                _logTextHandler.AddLog(t);
                yield return _logWaitForSeconds;
            }
            _soundHandler.PlaySe(_setKeyAudio);

            // 安全に範囲を制御
            var maxIndex = Mathf.Min(_setKeyCount + getKeyCount, _obeliskSides.Length);
    
            for (var i = _setKeyCount; i < maxIndex; i++)
            {
                _obeliskSideParticles[i].Play();
            }
    
            yield return _sideEffectWaitForSeconds;
    
            for (var i = _setKeyCount; i < maxIndex; i++)
            {
                _obeliskSides[i].SetActive(true);
            }
            yield return _camChangeForSeconds;
            _vCam.Priority = LowPriority;
            _playerClasHub.SetPlayerFreedom(true);
            _setKeyCount = maxIndex;
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
