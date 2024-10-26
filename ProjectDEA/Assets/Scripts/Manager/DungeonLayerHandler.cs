using System.Collections;
using Manager.Audio;
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
            if (nextScene.name == "ResultScene")
            {
                Destroy(gameObject);
                return; 
            }
            var logTextHandler = GameObject.FindWithTag("LogTextHandler").GetComponent<LogTextHandler>();
            logTextHandler.AddLog("また一つ降りられた。\n残るは" + CurrentLayer +"Fだ。");
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