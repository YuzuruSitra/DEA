using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class DungeonLayerHandler : MonoBehaviour
    {
        public int CurrentLayer { get; private set; }
        [SerializeField] private int _maxLayer;
        
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
            SceneManager.LoadScene("ResultScene");
        }
        
    }
}