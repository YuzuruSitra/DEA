using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class TitleBtHandler : MonoBehaviour
    {
        [SerializeField] private Button _startBt;

        private void Start()
        {
            _startBt.onClick.AddListener(NextScene);
        }

        private static void NextScene()
        {
            SceneManager.LoadScene("DungeonStart");
        }
    }
}
