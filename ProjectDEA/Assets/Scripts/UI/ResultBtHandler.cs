using Manager.Audio;
using Manager.MetaAI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class ResultBtHandler : MonoBehaviour
    {
        [SerializeField] private Button _exitBt;
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _pushAudio;
        
        private MetaAIHandler _metaAIHandler;
        
        private void Start()
        {
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
            _metaAIHandler = GameObject.FindWithTag("MetaAI").GetComponent<MetaAIHandler>();
            _exitBt.onClick.AddListener(ExitGame);
        }

        private void ExitGame()
        {
            _soundHandler.PlaySe(_pushAudio);
            _metaAIHandler.ResetMetaAI();
            SceneManager.LoadScene("TitleScene");
        }
    }
}
