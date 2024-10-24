using System;
using System.Collections;
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
                SceneManager.LoadScene("DungeonIn");
                return;
            }
            IsGameClear = true;
            SceneManager.LoadScene("ResultScene");
        }
        
        void SceneLoaded (Scene nextScene, LoadSceneMode mode)
        {
            if (CurrentLayer == _maxLayer) return;
            var logTextHandler = GameObject.FindWithTag("LogTextHandler").GetComponent<LogTextHandler>();
            logTextHandler.AddLog("また一つ降りられた。\n残るは" + CurrentLayer +"Fだ。");
        }

        public void PlayerDeathNext()
        {
            IsGameClear = false;
            StartCoroutine(WaitForDeathNext());
        }

        private IEnumerator WaitForDeathNext()
        {
            yield return new WaitForSeconds(4.0f);
            SceneManager.LoadScene("ResultScene");
        }
    }
}