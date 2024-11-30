using System.Collections;
using Character.Player;
using Cinemachine;
using Gimmick;
using Manager.Audio;
using Manager.Language;
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

            AddLocalizedLog(
                "ようやく" + _dungeonLayerHandler.CurrentLayer + "Fまで降りられた。",
                "I've finally made it down to the " + _dungeonLayerHandler.CurrentLayer + "nd floor."
            );
            
            yield return WaitForPlayerInput(1);
            
            AddLocalizedLog(
                "このダンジョンは息苦しくて持続的にダメージを受けている気がする。",
                "This dungeon feels suffocating, like I'm taking continuous damage just by being here."
            );
            
            yield return WaitForPlayerInput(2);
            
            AddLocalizedLog(
                "オベリスクの転移による疲労回復がなかったら今頃...",
                "If it weren't for the recovery from the obelisk's teleportation,\nI don't know where I'd be by now..."
            );
            
            yield return WaitForPlayerInput(3);
            
            AddLocalizedLog(
                "これまで通り転移ギミックを利用した脱出を試みよう。",
                "I'll try to escape using the teleportation gimmick, just like before."
            );
            
            yield return WaitForPlayerInput(4);

            ChangeVCam(1);
            
            AddLocalizedLog(
                "ちょうどオベリスクがある。\nたしか起動には欠片が" + ExitObelisk.NeededKeyCount + "つ必要だったはずだ。",
                "There's an obelisk right here.\nIf I remember correctly, activating it requires " + ExitObelisk.NeededKeyCount + " obelisk fragments."
            );
            
            yield return WaitForPlayerInput(5);

            ChangeVCam(0);
            yield return WaitForPlayerInput(6);
            
            _tutorialIndicationUI.SetActive(false);
            
            _panelSwitcher.ChangeIsManipulate(true);
            _playerClasHub.SetPlayerFreedom(true);
            _logTextHandler.AllOnDisableTMPro();
            _isTutorial = false;
        }
        
        private void AddLocalizedLog(string japaneseMessage, string englishMessage)
        {
            var language = _logTextHandler.LanguageHandler.CurrentLanguage;
            var message = language switch
            {
                LanguageHandler.Language.Japanese => japaneseMessage,
                LanguageHandler.Language.English => englishMessage,
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(message))
            {
                _logTextHandler.AddLog(message, false);
            }
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