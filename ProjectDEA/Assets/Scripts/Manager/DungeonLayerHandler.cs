using System.Collections;
using Manager.Audio;
using Manager.Language;
using Manager.PlayData;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class DungeonLayerHandler : MonoBehaviour
    {
        public int CurrentLayer { get; private set; }
        [SerializeField] private int _maxLayer;
        public bool IsGameClear { get; private set; }
        private AnalysisDataHandler _analysisDataHandler;
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _nextLayerAudio;
        
        private void Awake()
        {
            CheckSingleton();
        }

        private void CheckSingleton()
        {
            var target = GameObject.FindGameObjectWithTag(gameObject.tag);
            var checkResult = target != null && target != gameObject;

            if (checkResult)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += SceneLoaded;
            CurrentLayer = _maxLayer;
            _analysisDataHandler = GameObject.FindWithTag("AnalysisDataHandler").GetComponent<AnalysisDataHandler>();
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }

        public void NextDungeonLayer()
        {
            if (CurrentLayer > 1)
            {
                CurrentLayer--;
                _soundHandler.PlaySe(_nextLayerAudio);
                SceneManager.LoadScene("DungeonIn");
                return;
            }
            IsGameClear = true;
            _analysisDataHandler.IsClear = true;
            SceneManager.LoadScene("ResultScene");
        }
        
        private void SceneLoaded (Scene nextScene, LoadSceneMode mode)
        {
            switch (nextScene.name)
            {
                case "ResultScene":
                    Destroy(gameObject);
                    return;
                case "DungeonStart":
                    return;
                default:
                {
                    var logTextHandler = GameObject.FindWithTag("LogTextHandler").GetComponent<LogTextHandler>();
                    var language = logTextHandler.LanguageHandler.CurrentLanguage;
                    if (CurrentLayer == _maxLayer - 1)
                    {
                        switch (language)
                        {
                            case LanguageHandler.Language.Japanese:
                                logTextHandler.AddLog("ここがダンジョンか。とりあえず探索しつつオベリスクを目指そう。");
                                break;
                            case LanguageHandler.Language.English:
                                logTextHandler.AddLog("So this is the dungeon. Let's just explore and aim for the obelisk.");
                                break;
                        }
                    }
                    else
                    {
                        switch (language)
                        {
                            case LanguageHandler.Language.Japanese:
                                logTextHandler.AddLog("転送されたようだ。\n残るは" + CurrentLayer + "F程か。");
                                break;
                            case LanguageHandler.Language.English:
                                logTextHandler.AddLog("I managed to descend another floor.\nOnly the " + CurrentLayer +
                                                      "nd floor remains.");
                                break;
                        }
                    }
                    break;
                }
            }
        }

        public void PlayerDeathNext()
        {
            IsGameClear = false;
            _analysisDataHandler.IsClear = false;
            StartCoroutine(WaitForDeathNext());
        }

        private IEnumerator WaitForDeathNext()
        {
            yield return new WaitForSeconds(4.0f);
            SceneManager.LoadScene("ResultScene");
        }
    }
}