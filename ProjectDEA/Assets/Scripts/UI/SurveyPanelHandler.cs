using Manager.Audio;
using Manager.PlayData;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SurveyPanelHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _surveyPanel1;
        [SerializeField] private GameObject _surveyPanel2;
        [SerializeField] private Button _nextPanelBt;
        [SerializeField] private Button _beforePanelBt;
        [SerializeField] private Button _submitBt;
        [SerializeField] private Slider[] _answerSliders;
        private AnalysisDataHandler _analysisDataHandler;
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _pushAudio;
        
        private void Start()
        {
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
            _analysisDataHandler = GameObject.FindWithTag("AnalysisDataHandler").GetComponent<AnalysisDataHandler>();
            _beforePanelBt.onClick.AddListener(SwitchSurveyPanel);
            _nextPanelBt.onClick.AddListener(SwitchSurveyPanel);
            _submitBt.onClick.AddListener(SubmitQuestions);
        }

        private void SwitchSurveyPanel()
        {
            _soundHandler.PlaySe(_pushAudio);
            _surveyPanel1.SetActive(!_surveyPanel1.activeSelf);
            _surveyPanel2.SetActive(!_surveyPanel2.activeSelf);
        }

        private void SubmitQuestions()
        {
            _soundHandler.PlaySe(_pushAudio);
            _analysisDataHandler.SaveAnswerSet(_answerSliders);
        }
        
    }
}
