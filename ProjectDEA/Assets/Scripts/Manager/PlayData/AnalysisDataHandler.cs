using System;
using System.Collections.Generic;
using Gimmick;
using Manager.Map;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager.PlayData
{
    public class AnalysisDataHandler : MonoBehaviour
    {
        private PlayDataWriter _dataWriter;
        public int PlayerID { get; set; }
        private bool _isAddListener;
        
        public bool IsClear { get; set; }
        private const string ClearKey = "ClearKey";
        
        private float _clearTime;
        private const string ClearTimeKey = "ClearTimeKey";
        private bool _isCountUp;
        
        private int _roomMovementCount;
        private const string MovementCountKey = "MovementCountKey";
        private InRoomChecker _inRoomChecker;
        private Transform _playerTransform;
        private int _playerCurrentRoom;
        
        private Dictionary<GimmickGenerator.GimmickKind, int> _gimmicksCount;
        private const string GimmicksCountKey = "CountKey";
        
        public int EnemyKillCount { get; set; }
        private const string EnemyKillCountKey = "EnemyKillCountKey";
        public int PickedBonesCount { get; set; }
        private const string PickedBonesCountKey = "PickedBonesCountKey";
        public int DestroyObjCount { get; set; }
        private const string DestroyObjCountKey = "DestroyObjCountKey";
        
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
            // _gimmicksCountを初期化
            _gimmicksCount = new Dictionary<GimmickGenerator.GimmickKind, int>();

            // GimmickKindの全要素を取得し、辞書に初期値0で格納
            foreach (GimmickGenerator.GimmickKind kind in Enum.GetValues(typeof(GimmickGenerator.GimmickKind)))
            {
                _gimmicksCount[kind] = 0;
            }
            SceneManager.sceneLoaded += SceneLoaded;
            _isAddListener = true;
        }
        
        private void OnDestroy()
        {
            if(!_isAddListener) return;
            SceneManager.sceneLoaded -= SceneLoaded;
        }

        private void Update()
        {
            if (_isCountUp) _clearTime += Time.deltaTime;

            if (_inRoomChecker != null && _playerTransform != null)
            {
                var roomNum = _inRoomChecker.CheckStayRoomNum(_playerTransform.transform.position);
                if (roomNum == InRoomChecker.ErrorRoomNum) return;
                if (_playerCurrentRoom == roomNum) return;
                _roomMovementCount++;
                _playerCurrentRoom = roomNum;
            }
        }
        
        private void SceneLoaded (Scene nextScene, LoadSceneMode mode)
        {
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
                    _inRoomChecker = new InRoomChecker();
                    _playerTransform = GameObject.FindWithTag("Player").transform;
                    break;
                case "ResultScene":
                    _isCountUp = false;
                    SavePlayLog();
                    _inRoomChecker = null;
                    _playerTransform = null;
                    break;
            }
        }

        private void ResetCount()
        {
            IsClear = false;
            _clearTime = 0;
            _isCountUp = false;
            _roomMovementCount = 0;

            foreach (GimmickGenerator.GimmickKind kind in Enum.GetValues(typeof(GimmickGenerator.GimmickKind)))
            {
                _gimmicksCount[kind] = 0;
            }
            EnemyKillCount = 0;
            PickedBonesCount = 0;
            DestroyObjCount = 0;
        }

        private void SavePlayLog()
        {
            _dataWriter.SaveIsClear(ClearKey, IsClear);
            var cropTime = Mathf.Round(_clearTime * 10) / 10f;
            _dataWriter.SaveClearTime(ClearTimeKey, cropTime);
            _dataWriter.SaveMovementCount(MovementCountKey, _roomMovementCount);
            _dataWriter.SaveGimmicksCount(GimmicksCountKey, _gimmicksCount);
            _dataWriter.SaveEnemyKillCount(EnemyKillCountKey, EnemyKillCount);
            _dataWriter.SavePickedBonesCount(PickedBonesCountKey, PickedBonesCount);
            _dataWriter.SaveDestroyObjCount(DestroyObjCountKey, DestroyObjCount);
        }

        public void ChangeGimmicksCount(GimmickGenerator.GimmickKind insKind)
        {
            foreach (GimmickGenerator.GimmickKind kind in Enum.GetValues(typeof(GimmickGenerator.GimmickKind)))
            {
                if (insKind != kind) continue;
                _gimmicksCount[kind]++;
            }
        }
    }
}
