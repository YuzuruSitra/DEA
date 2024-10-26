using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager.Audio
{
    public class BGMSwitcher : MonoBehaviour
    {
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip[] _audioClip;
        private bool _isAddedListener;
        private void Start()
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
            _isAddedListener = true;
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
            _soundHandler.PlayBGM(_audioClip[0]);
        }

        private void OnDestroy()
        {
            if (!_isAddedListener) return;
            SceneManager.sceneLoaded -= SceneLoaded;
        }

        private void SceneLoaded (Scene nextScene, LoadSceneMode mode)
        {
            switch (nextScene.name)
            {
                case "TitleScene":
                    _soundHandler.PlayBGM(_audioClip[0]);
                    break;
                case "DungeonStart":
                case "DungeonIn":
                    _soundHandler.PlayBGM(_audioClip[1]);
                    break;
                case "ResultScene":
                    _soundHandler.PlayBGM(_audioClip[2]);
                    break;
            }
        }
    }
}
