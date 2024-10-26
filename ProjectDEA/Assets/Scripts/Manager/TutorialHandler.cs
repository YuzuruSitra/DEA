using System.Collections;
using Character.Player;
using Cinemachine;
using Gimmick;
using Manager.Audio;
using UI;
using UnityEngine;

namespace Manager
{
    public class TutorialHandler : MonoBehaviour
    {
        [SerializeField] private DungeonLayerHandler _dungeonLayerHandler;
        [SerializeField] private LogTextHandler _logTextHandler;
        [SerializeField] private PlayerClasHub _playerClasHub;
        [SerializeField] private PanelSwitcher _panelSwitcher;
        [SerializeField] private CinemachineVirtualCameraBase[] _vCams;
        [SerializeField] private float _inputPaddingTime;
        private float _inputCurrentTime;
        private const int LowPriority = 5;
        private const int HighPriority = 10;
        private int _currentWaitState;
        [SerializeField] private GameObject _tutorialIndicationUI;
        private bool _isIndicationActive;
        private bool _isTutorial;
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _pushAudio;
        
        private void Start()
        {
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
            StartCoroutine(TutorialCoroutine());
        }

        private void Update()
        {
            if (!_isTutorial) return;
            var shouldShowUI = _inputCurrentTime >= _inputPaddingTime;

            if (shouldShowUI != _isIndicationActive)
            {
                _tutorialIndicationUI.SetActive(shouldShowUI);
                _isIndicationActive = shouldShowUI;
            }

            if (_inputCurrentTime < _inputPaddingTime)
            {
                _inputCurrentTime += Time.deltaTime;
                return;
            }

            if (!Input.anyKeyDown) return;
            _currentWaitState++;
            _inputCurrentTime = 0;
            _soundHandler.PlaySe(_pushAudio);
        }

        private IEnumerator TutorialCoroutine()
        {
            _isTutorial = true;
            _panelSwitcher.ChangeIsManipulate(false);
            _playerClasHub.SetPlayerFreedom(false);
            var message1 = "ようやく" + _dungeonLayerHandler.CurrentLayer + "Fまで降りられた。";
            _logTextHandler.AddLog(message1, false);
            yield return WaitForPlayerInput(1);

            var message2 = "これまで通りオベリスクの転移を利用して脱出を目指そう。";
            _logTextHandler.AddLog(message2, false);
            yield return WaitForPlayerInput(2);

            ChangeVCam(1);
            var message3 = "ちょうどオベリスクがある。\nたしか起動には欠片が" + ExitObelisk.NeededKeyCount + "つ必要だったはずだ。";
            _logTextHandler.AddLog(message3, false);
            yield return WaitForPlayerInput(3);

            ChangeVCam(0);
            yield return WaitForPlayerInput(4);
            
            _tutorialIndicationUI.SetActive(false);
            
            _panelSwitcher.ChangeIsManipulate(true);
            _playerClasHub.SetPlayerFreedom(true);
            _logTextHandler.AllOnDisableTMPro();
            _isTutorial = false;
        }
        
        private IEnumerator WaitForPlayerInput(int target)
        {
            while (_currentWaitState < target)　yield return null;
        }

        private void ChangeVCam(int target)
        {
            for (var i = 0; i < _vCams.Length; i++)
            {
                var cam = _vCams[i];
                cam.Priority = target == i ? HighPriority : LowPriority;
            }
        }
    }
}
