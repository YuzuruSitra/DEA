using Manager.Map;
using Manager.MetaAI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Manager.PlayData
{
    public class AnalysisDataHandler : MonoBehaviour
    {
        private PlayDataWriter _dataWriter;
        private bool _isAddListener;

        private int _playerID;
        public int PlayerID
        {
            get => _playerID;
            set
            {
                if (_playerID == value) return; // 同じ値の場合は処理しない
                _playerID = value;
                if (_dataWriter != null) // _dataWriterが初期化されている場合のみ実行
                {
                    _dataWriter.SetPlayerSlot(_playerID);
                }
            }
        }

        // 他のプロパティとメンバ変数
        public bool IsClear { get; set; }
        private const string ClearKey = "ClearKey";
        private float _clearTime;
        private const string ClearTimeKey = "ClearTimeKey";
        private bool _isCountUp;
        private int _roomMovementCount;
        private const string MovementCountKey = "MovementCountKey";
        private PlayerRoomTracker _playerRoomTracker;
        private bool _isAddedRoomTracker;
        [SerializeField] private MetaAIHandler _metaAIHandler;
        private const string SaveIsUseMetaAIKey = "IsUseMetaAIKey";
        private const string ActionCountKey = "ActionCountKey";
        private int _actionCount;
        private const string PlayerTypeKey = "PlayerTypeKey";
        private const string AnswerKey = "AnswerKey";

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
            _dataWriter = new PlayDataWriter();
            _dataWriter.SetPlayerSlot(_playerID);
            SceneManager.sceneLoaded += SceneLoaded;
            _isAddListener = true;
            _metaAIHandler.OnAddEvent += AddActionCount;
        }

        private void OnDestroy()
        {
            if (!_isAddListener) return;
            SceneManager.sceneLoaded -= SceneLoaded;
            _metaAIHandler.OnAddEvent -= AddActionCount;
        }

        private void Update()
        {
            if (_isCountUp) _clearTime += Time.deltaTime;
        }

        private void AddRoomChangeCount()
        {
            _roomMovementCount++;
        }

        private void SceneLoaded(Scene nextScene, LoadSceneMode mode)
        {
            if (_isAddedRoomTracker)
            {
                _playerRoomTracker.OnPlayerRoomChange -= AddRoomChangeCount;
                _isAddedRoomTracker = false;
            }
            switch (nextScene.name)
            {
                case "TitleScene":
                    ResetCount();
                    break;
                case "DungeonStart":
                    break;
                case "DungeonIn":
                    _isCountUp = true;
                    _playerRoomTracker = GameObject.FindWithTag("PlayerRoomTracker").GetComponent<PlayerRoomTracker>();
                    _playerRoomTracker.OnPlayerRoomChange += AddRoomChangeCount;
                    _isAddedRoomTracker = true;
                    break;
                case "ResultScene":
                    _isCountUp = false;
                    SavePlayLog();
                    _playerRoomTracker = null;
                    break;
            }
        }

        private void AddActionCount()
        {
            _actionCount++;
        }

        private void ResetCount()
        {
            IsClear = false;
            _clearTime = 0;
            _isCountUp = false;
            _roomMovementCount = 0;
            _actionCount = 0;
        }

        private void SavePlayLog()
        {
            _dataWriter.SaveIsClear(ClearKey, IsClear);
            _dataWriter.SaveIsUseMetaAI(SaveIsUseMetaAIKey, _metaAIHandler.IsUse);
            var cropTime = Mathf.Round(_clearTime * 10) / 10f;
            _dataWriter.SaveClearTime(ClearTimeKey, cropTime);
            _dataWriter.SaveMovementCount(MovementCountKey, _roomMovementCount);
            _dataWriter.SaveActionCount(ActionCountKey, _actionCount);
            _dataWriter.SavePlayerType(PlayerTypeKey, _metaAIHandler.CurrentPlayerType.ToString());
        }

        public void SaveAnswerSet(Slider[] answers)
        {
            for (var i = 0; i < answers.Length; i++)
            {
                var key = AnswerKey + i;
                var answer = (int)answers[i].value;
                _dataWriter.SaveAnswers(key, answer);
            }
        }
    }
}
