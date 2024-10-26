using Manager.Audio;
using Manager.PlayData;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class TitleUIHandler : MonoBehaviour
    {
        [SerializeField] private Button _startBt;
        [SerializeField] private Button _devBt;
        [SerializeField] private GameObject _devPad;
        [SerializeField] private Button _increaseIdBt;
        [SerializeField] private Button _reduceIDBt;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private TextMeshProUGUI _idText;
        [SerializeField] private AnalysisDataHandler _analysisData;
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _pushAudio;
        
        private void Start()
        {
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
            _startBt.onClick.AddListener(NextScene);
            _devBt.onClick.AddListener(ChangeDevPad);
            _increaseIdBt.onClick.AddListener(IncreasePlayerID);
            _reduceIDBt.onClick.AddListener(ReducePlayerID);
            _inputField.onEndEdit.AddListener(SetPlayerID);
            ChangeIdText();
        }

        private void NextScene()
        {
            SceneManager.LoadScene("DungeonStart");
            _soundHandler.PlaySe(_pushAudio);
        }

        private void ChangeDevPad()
        {
            var newState = !_devPad.activeSelf;
            _devPad.SetActive(newState);
            _soundHandler.PlaySe(_pushAudio);
        }

        private void IncreasePlayerID()
        {
            _analysisData.PlayerID++;
            ChangeIdText();
            _soundHandler.PlaySe(_pushAudio);
        }

        private void ReducePlayerID()
        {
            if (_analysisData.PlayerID == 0) return;
            _analysisData.PlayerID--;
            ChangeIdText();
            _soundHandler.PlaySe(_pushAudio);
        }

        private void SetPlayerID(string value)
        {
            if (!int.TryParse(value, out var inputValue)) return;
            _analysisData.PlayerID = inputValue;
            ChangeIdText(false);
        }

        private void ChangeIdText(bool changeTextField = true)
        {
            _idText.text = "プレイヤーID: " + _analysisData.PlayerID;
            if (!changeTextField) return;
            _inputField.text = "" + _analysisData.PlayerID;
        }
        
    }
}
