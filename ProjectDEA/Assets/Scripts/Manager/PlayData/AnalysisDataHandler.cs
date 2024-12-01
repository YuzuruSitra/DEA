using Manager.Map;
using Manager.MetaAI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager.PlayData
{
    public class AnalysisDataHandler : MonoBehaviour
    {
        private PlayDataWriter _dataWriter;
        public int PlayerID { get; set; }
        private bool _isAddListener;
        
        // クリア判定
        public bool IsClear { get; set; }
        private const string ClearKey = "ClearKey";
        // クリア時間
        private float _clearTime;
        private const string ClearTimeKey = "ClearTimeKey";
        private bool _isCountUp;
        // 移動回数
        private int _roomMovementCount;
        private const string MovementCountKey = "MovementCountKey";
        private PlayerRoomTracker _playerRoomTracker;
        private bool _isAddedRoomTracker;
        // アクション回数
        [SerializeField] private MetaAIHandler _metaAIHandler;
        private const string ActionCountKey = "ActionCountKey";
        private int _actionCount;
        // プレイヤータイプの保存
        private const string PlayerTypeKey = "PlayerTypeKey";
        
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
            SceneManager.sceneLoaded += SceneLoaded;
            _isAddListener = true;
            _metaAIHandler.OnAddEvent += AddActionCount;
        }
        
        private void OnDestroy()
        {
            if(!_isAddListener) return;
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
            // リスナー解除
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
                    _dataWriter.SetPlayerSlot(PlayerID);
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
            var cropTime = Mathf.Round(_clearTime * 10) / 10f;
            _dataWriter.SaveClearTime(ClearTimeKey, cropTime);
            _dataWriter.SaveMovementCount(MovementCountKey, _roomMovementCount);
            _dataWriter.SaveActionCount(ActionCountKey, _actionCount);
            _dataWriter.SavePlayerType(PlayerTypeKey, _metaAIHandler.CurrentPlayerType.ToString());
        }
    }
}
