using System.Collections;
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
            CurrentLayer = _maxLayer;
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