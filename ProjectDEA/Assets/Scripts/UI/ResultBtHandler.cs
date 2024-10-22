using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class ResultBtHandler : MonoBehaviour
    {
        [SerializeField] private Button _exitBt;

        private void Start()
        {
            _exitBt.onClick.AddListener(ExitGame);
        }

        private static void ExitGame()
        {
            SceneManager.LoadScene("TitleScene");
        }
    }
}
