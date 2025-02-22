using System.Collections;
using Character.Player;
using Cinemachine;
using Item;
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
        private bool _isTutorialLog;
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _pushAudio;
        [SerializeField] private TutorialArea1 _tutorialArea1;
        [SerializeField] private InventoryHandler _inventoryHandler;
        [SerializeField] private float _logBombTime;
        private WaitForSeconds _logBombTimeSeconds;
        [SerializeField] private float _logFadeOutTime;
        private WaitForSeconds _logFadeOutTimeSeconds;
        [SerializeField] private Transform _targetTransform; 
        [SerializeField] private Transform _playerTransform;
        
        private void Start()
        {
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
            _logBombTimeSeconds = new WaitForSeconds(_logBombTime);
            _logFadeOutTimeSeconds = new WaitForSeconds(_logFadeOutTime);
            StartCoroutine(TutorialCoroutine());
        }

        private void Update()
        {
            if (!_isTutorialLog) return;
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
            _isTutorialLog = true;
            _panelSwitcher.ChangeIsManipulate(false);
            _playerClasHub.SetPlayerFreedom(false);

            AddLocalizedLog(
                "集落で、妙な噂を耳にした。",
                "In the village, I heard a strange rumor."
            );
            
            yield return WaitForPlayerInput(1);
            
            AddLocalizedLog(
                "鉱夫たちが次々と姿を消し、誰一人戻ってこないという。",
                "The miners disappeared one by one and none of them returned."
            );
            
            yield return WaitForPlayerInput(2);
            
            AddLocalizedLog(
                "痕跡を辿り、薄暗い岩壁を抜けた先で、私はそれを見つけた。",
                "I followed the trail and found it through a dimly lit rock wall."
            );
            
            yield return WaitForPlayerInput(3);

            ChangeVCam(1);
            
            AddLocalizedLog(
                "黒曜石のように鈍く輝く、見たこともない異形の石柱__オベリスク。",
                "An oddly shaped stone pillar __ obelisk, which shines as dully as obsidian and has never been seen before."
            );
            yield return WaitForPlayerInput(4);
            ChangeVCam(0);
            
            AddLocalizedLog(
                "まずは手前の岩をなんとかするか。",
                "First, let's deal with the rocks in the foreground."
            );
            yield return WaitForPlayerInput(5);
            
            _tutorialIndicationUI.SetActive(false);
            _isTutorialLog = false;
            _tutorialArea1.gameObject.SetActive(true);
            var playerMover = _playerClasHub.PlayerMover;
            playerMover.SetWalkableState(true);
            AddLocalizedLog(
                "[ 赤いエリアへ向かおう ]",
                "[ Let's head to the red area. ]"
            );
            while (!_tutorialArea1.IsReaching)
            {
                yield return null;
            }
            
            _tutorialArea1.gameObject.SetActive(false);
            AddLocalizedLog(
                "[ 岩を攻撃してみよう ]",
                "[ Let's attack the rock. ]"
            );
            playerMover.SetWalkableState(false);
            var playerAttackHandler = _playerClasHub.PlayerAttackHandler;
            playerAttackHandler.SetCanAttackState(true);
            while (!playerAttackHandler.IsAttacking)
            {
                yield return null;
            }
            
            _tutorialIndicationUI.SetActive(true);
            _isTutorialLog = true;
            AddLocalizedLog(
                "さすがに剣じゃきびしいな...",
                "I'm not sure I can do it with a sword..."
            );
            playerAttackHandler.SetCanAttackState(false);
            yield return WaitForPlayerInput(6);
            
            AddLocalizedLog(
                "宝箱がある。なにかないかな...",
                "There is a treasure chest. Let's see if there's anything..."
            );
            yield return WaitForPlayerInput(7);
            
            _tutorialIndicationUI.SetActive(false);
            _isTutorialLog = false;
            
            playerMover.SetWalkableState(true);
            var playerInteraction = _playerClasHub.PlayerInteraction;
            playerInteraction.SetInteractableState(true);
            
            while (_inventoryHandler.ItemSets[(int)ItemKind.Dynamite]._count == 0)
            {
                yield return null;
            }
            
            // 方向転換
            playerMover.SetWalkableState(false);
            _playerTransform.LookAt(_targetTransform);
            var playerUseItem = _playerClasHub.PlayerUseItem;
            playerUseItem.SetCanUseItemState(true);
            AddLocalizedLog(
                "さっき手に入れたダイナマイトを使おう。",
                "Let's use the dynamite we just got."
            );
            while (_inventoryHandler.ItemSets[(int)ItemKind.Dynamite]._count == 1)
            {
                yield return null;
            }
            
            AddLocalizedLog(
                "爆発しそうだ。すぐに逃げよう。",
                "It's going to explode. Let's get out of here right away."
            );
            _playerClasHub.SetPlayerFreedom(true);

            yield return _logBombTimeSeconds;
            
            AddLocalizedLog(
                "ようやくオベリスクへ向かえる。",
                "At last, we are approaching the obelisk."
            );

            yield return _logFadeOutTimeSeconds;
            
            _tutorialIndicationUI.SetActive(false);
            
            _panelSwitcher.ChangeIsManipulate(true);
            _isTutorialLog = false;
            _logTextHandler.AllOnDisableTMPro();
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