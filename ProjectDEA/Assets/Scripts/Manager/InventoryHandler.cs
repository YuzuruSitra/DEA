using System;
using Item;
using Manager.Audio;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class InventoryHandler : MonoBehaviour
    {
        [Serializable]
        public struct ItemPrefabSet
        {
            public ItemKind _kind;
            public string[] _name;
            public string[] _description;
            public GameObject _prefab;
            public GameObject _currentPredict;
            public GameObject _predict;
            public Sprite _sprite;
            public int _count;
            public bool _isPut;
            public bool _isUse;
            public string[] _effectedLogText;
        }
        [SerializeField] private ItemPrefabSet[] _itemSets;
        [SerializeField] private ItemKind[] _onlyMissionItems;
        public ItemPrefabSet[] ItemSets => _itemSets;
        public const int ErrorValue = -1;
        public int CurrentItemNum { get; private set; }
        public Action OnItemNumChanged;
        public Action<Sprite> OnItemSpriteChanged;
        public Action<int> OnItemCountChanged;
        public Action<ItemPrefabSet[]> OnItemLineupChanged;
        public ItemPrefabSet TargetItem => _itemSets[CurrentItemNum];
        private LogTextHandler _logTextHandler;
        private readonly string[] _getLogTemplate =
        {
            "を手に入れた。",
            " has been obtained."
        };
        private readonly string[] _lostLogTemplate =
        {
            "を失った。",
            " are gone."
        };
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _pushAudio;
        [SerializeField] private AudioClip _getItemAudio;
        private InputActions _inputActions;
        private bool[] _putSignCandle;

        public const int InfiniteNum = -1;
        [Serializable]
        public struct MemoirsData
        {
            public string _title;
            public string _content;
            public bool _active;
        } 
        [SerializeField] private MemoirsData[] _memoirsDataSet;
        public MemoirsData[] MemoirsDataSet => _memoirsDataSet;
        public Action OnMemoirsChanged;
        
        private void Awake()
        {
            CheckSingleton();
            SceneManager.sceneLoaded += OnSceneLoaded;
            // Interactアクションのイベントリスナーを登録
            _inputActions = new InputActions();
            _inputActions.Player.InventryShift.performed += IncreaseItemNum;
            _inputActions.Enable();
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _inputActions.Player.InventryShift.performed -= IncreaseItemNum;
            _inputActions.Disable();
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (SceneManager.GetActiveScene().name == "ResultScene")
            {
                Destroy(gameObject);
                return;
            }
            OnInitial();
        }

        private void OnInitial()
        {
            _logTextHandler = GameObject.FindWithTag("LogTextHandler").GetComponent<LogTextHandler>();
            // Setting predict.
            for (var i = 0; i < _itemSets.Length; i++)
            {
                if (_itemSets[i]._predict == null) continue;
                _itemSets[i]._currentPredict = Instantiate(_itemSets[i]._predict);
            }
            
            CurrentItemNum = ErrorValue;
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
            
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
        }

        private void IncreaseItemNum(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            var startIndex = Math.Max(CurrentItemNum, 0);

            for (var i = 1; i <= _itemSets.Length; i++)
            {
                var newIndex = (startIndex + i) % _itemSets.Length;
                
                if (_itemSets[newIndex]._count <= 0 && _itemSets[newIndex]._count != InfiniteNum) continue;
                ChangeItemNum(newIndex);
                if (CurrentItemNum != newIndex) _soundHandler.PlaySe(_pushAudio);
                return;
            }

            ChangeItemNum(ErrorValue);
        }

        private void ChangeItemNum(int value)
        {
            if (CurrentItemNum != ErrorValue)
            {
                ChangePredictActive(_itemSets[CurrentItemNum]._currentPredict, false);
            }
            CurrentItemNum = value;
            var sprite = CurrentItemNum != ErrorValue ? _itemSets[value]._sprite : null;
            OnItemSpriteChanged?.Invoke(sprite);
            OnItemNumChanged?.Invoke();
            ChangeItemCount();
        }

        private void ChangeItemCount()
        {
            if (CurrentItemNum == ErrorValue)
            {
                OnItemCountChanged?.Invoke(0);
                return;
            }
            OnItemCountChanged?.Invoke(_itemSets[CurrentItemNum]._count);
        }

        // アイテムをインベントリに追加する
        public void AddItem(ItemKind item)
        {
            var language = _logTextHandler.LanguageHandler.CurrentLanguage;
            for (var i = 0; i < _itemSets.Length; i++)
            {
                if (_itemSets[i]._kind != item) continue;
                if (_itemSets[i]._kind == ItemKind.RaggedMemoirs)
                {
                    if (_itemSets[i]._count == 0)
                    {
                        _itemSets[i]._count = InfiniteNum;
                        OnItemLineupChanged(_itemSets);
                    }
                    var startIndex = UnityEngine.Random.Range(0, _memoirsDataSet.Length);
                    for (var j = 0; j < _memoirsDataSet.Length; j++)
                    {
                        var index = (startIndex + j) % _memoirsDataSet.Length;
                        if (_memoirsDataSet[index]._active) continue;
                        _memoirsDataSet[index]._active = true;
                        OnMemoirsChanged?.Invoke();
                        break;
                    }
                }
                if (_itemSets[i]._count != InfiniteNum)
                {
                    _itemSets[i]._count++;
                }
                var message = _itemSets[i]._name[(int)language] + _getLogTemplate[(int)language];
                _logTextHandler.AddLog(message);
                _soundHandler.PlaySe(_getItemAudio);
                if (CurrentItemNum == i) ChangeItemCount();
                if (_itemSets[i]._count == 1) OnItemLineupChanged(_itemSets);
                if (CurrentItemNum == ErrorValue) ChangeItemNum(i);
                break;
            }
        }

        public void RemoveMissionItem()
        {
            var language = _logTextHandler.LanguageHandler.CurrentLanguage;
            for (var i = 0; i < _itemSets.Length; i++)
            {
                foreach (var t in _onlyMissionItems)
                {
                    if (_itemSets[i]._kind != t) continue;
                    if (_itemSets[i]._count <= 0) continue;
                    _itemSets[i]._count = 0;
                    var message = _itemSets[i]._name[(int)language] + _lostLogTemplate[(int)language];
                    _logTextHandler.AddLog(message);
                }
            }
        }

        public void UseItem()
        {
            switch (CurrentItemNum)
            {
                case (int)ItemKind.SignCandle:
                case (int)ItemKind.RaggedMemoirs:
                case ErrorValue:
                    return;
            }
            
            _itemSets[CurrentItemNum]._count = Math.Max(0, _itemSets[CurrentItemNum]._count - 1);
            ChangeItemCount();
            
            if (_itemSets[CurrentItemNum]._count > 0) return;
            OnItemLineupChanged(_itemSets);
            ChangeItemNum(ErrorValue);
        }

        public void ChangePredictActive(GameObject predict, bool isVisible)
        {
            if (predict == null) return;
            predict.SetActive(isVisible);
        }
        
    }
}