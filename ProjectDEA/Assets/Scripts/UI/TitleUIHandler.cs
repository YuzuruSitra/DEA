using System;
using Manager.Audio;
using Manager.Language;
using Manager.MetaAI;
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
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private TextMeshProUGUI _idText;
        [SerializeField] private AnalysisDataHandler _analysisData;
        
        [SerializeField] private Button _settingBt;
        [SerializeField] private GameObject _languagePad;
        [SerializeField] private Button _languageIncreaseBt;
        [SerializeField] private TextMeshProUGUI _languageText;
        private LanguageHandler _languageHandler;
        
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _pushAudio;
        
        private MetaAIHandler _metaAIHandler;
        
        private void Start()
        {
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
            _languageHandler = GameObject.FindWithTag("LanguageHandler").GetComponent<LanguageHandler>();
            _metaAIHandler = GameObject.FindWithTag("MetaAI").GetComponent<MetaAIHandler>();
            _startBt.onClick.AddListener(NextScene);
            _devBt.onClick.AddListener(ChangeDevPad);
            _increaseIdBt.onClick.AddListener(IncreasePlayerID);
            _inputField.onEndEdit.AddListener(SetPlayerID);
            
            _settingBt.onClick.AddListener(ChangeSettings);
            _languageIncreaseBt.onClick.AddListener(NextLanguage);
            ChangeIdText();
        }

        private void NextScene()
        {
            _soundHandler.PlaySe(_pushAudio);
            _metaAIHandler.LaunchMetaAI();
            SceneManager.LoadScene("DungeonStart");
        }

        private void ChangeDevPad()
        {
            var newState = !_devPad.activeSelf;
            _devPad.SetActive(newState);
            _soundHandler.PlaySe(_pushAudio);
        }

        private void ChangeSettings()
        {
            var newState = !_languagePad.activeSelf;
            _languagePad.SetActive(newState);
            _soundHandler.PlaySe(_pushAudio);
        }

        private void IncreasePlayerID()
        {
            _analysisData.PlayerID++;
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

        private void NextLanguage()
        {
            var languageCount = Enum.GetValues(typeof(LanguageHandler.Language)).Length;
            var nextNum = (int)_languageHandler.CurrentLanguage + 1;
            if (nextNum >= languageCount) nextNum = 0;
            _languageHandler.SetLanguage((LanguageHandler.Language)nextNum);
            _languageText.text = _languageHandler.CurrentLanguage.ToString();
            _soundHandler.PlaySe(_pushAudio);
        }
        
    }
}
