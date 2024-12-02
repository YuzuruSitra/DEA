using Manager.Audio;
using Manager.Language;
using Manager.PlayData;
using TMPro;
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
        [SerializeField] private TextMeshProUGUI[] _answerTexts;
        private AnalysisDataHandler _analysisDataHandler;
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _pushAudio;
        [SerializeField] private TextMeshProUGUI _questionSeem1;
        [SerializeField] private TextMeshProUGUI _questionSeem2;
        [SerializeField] private TextMeshProUGUI _question1;
        [SerializeField] private TextMeshProUGUI _question2;
        [SerializeField] private TextMeshProUGUI _question3;
        private LanguageHandler _languageHandler;
        
        private readonly string[] _jpQuestionSet =
        {
            "1, 他のプレイヤーと競うことは好きですか？",
            "2, ゲーム内での成果や称号を集めるのが好きですか？",
            "3, ゲーム内の隠されたエリアを見つけるのが楽しいですか？",
            "4, 自由に探索して何かを発見することが好きですか？",
            "5, ミッションを達成することにやりがいを感じますか？",
            "6, 強力な敵に挑戦することにやりがいを感じますか？",
            "1, 自分の好きな順番や方法で探索できていると感じましたか？",
            "2, ダンジョンのギミックに対して緊張感や喜びを感じましたか？",
            "3, ゲーム内でストレスを感じると思った場面は多かったですか？",
            "4, ダンジョンはスムーズに探索できたと感じましたか？",
            "5, ほかにも探索やギミックの攻略をしてみたいと感じましたか？"
        };
        
        private readonly string[] _enQuestionSet =
        {
            "1, Do you enjoy competing with other players?",
            "2, Do you like collecting achievements or titles in the game?",
            "3, Do you enjoy discovering hidden areas in the game?",
            "4, Do you like freely exploring and finding new things?",
            "5, Do you feel a sense of accomplishment from completing missions?",
            "6, Do you find it rewarding to challenge powerful enemies?",
            "1, Did you feel that you could explore in your preferred order or way?",
            "2, Did you feel tension or excitement when interacting with dungeon gimmicks?",
            "3, Did you frequently feel stressed during the game?",
            "4, Did you find it easy to explore the dungeons smoothly?",
            "5, Did you feel like exploring or solving more gimmicks in the game?"
        };

        private readonly string[] _questionSeems1Language =
        {
            "普段のゲームプレイ傾向について\n教えていただきたいです。",
            "I'd like to know about your usual gaming preferences."
        };
        
        private readonly string[] _questionSeems2Language =
        {
            "このゲームの感想について\n教えていただきたいです。",
            "Please share your thoughts\n on this game."
        };
        
        private readonly string[] _question1Language =
        {
            "いいえ",
            "No"
        };
        private readonly string[] _question2Language =
        {
            "どちらともいえない",
            "Neutral"
        };
        private readonly string[] _question3Language =
        {
            "はい",
            "Yes"
        };
        
        
        private void Start()
        {
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
            _analysisDataHandler = GameObject.FindWithTag("AnalysisDataHandler").GetComponent<AnalysisDataHandler>();
            _languageHandler = GameObject.FindWithTag("LanguageHandler").GetComponent<LanguageHandler>();
            _languageHandler.OnLanguageChanged += ChangeLanguage;
            ChangeLanguage(_languageHandler.CurrentLanguage);
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

        private void ChangeLanguage(LanguageHandler.Language language)
        {
            Debug.Log("Language changed to: " + language);
            var texts = language == LanguageHandler.Language.Japanese ? _jpQuestionSet : _enQuestionSet;
            for (var i = 0; i < _answerTexts.Length; i++)
            {
                _answerTexts[i].text = texts[i];
            }
            _questionSeem1.text = _questionSeems1Language[(int)language];
            _questionSeem2.text = _questionSeems2Language[(int)language];
            _question1.text = _question1Language[(int)language];
            _question2.text = _question2Language[(int)language];
            _question3.text = _question3Language[(int)language];
        }
        
    }
}
