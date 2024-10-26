using Manager.Audio;
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
        
        private void Start()
        {
            _exitBt.onClick.AddListener(ExitGame);
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
        }

        private void ExitGame()
        {
            SceneManager.LoadScene("TitleScene");
            _soundHandler.PlaySe(_pushAudio);
        }
    }
}
