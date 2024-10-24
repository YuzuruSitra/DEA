using System;
using Gimmick;
using Item;
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
            public string _name;
            public string _description;
            public GameObject _prefab;
            public GameObject _currentPrefab;
            public GameObject _predict;
            public Sprite _sprite;
            public int _count;
            public bool _isUse;
        }
        [SerializeField] private ItemPrefabSet[] _itemSets;
        public ItemPrefabSet[] ItemSets => _itemSets;
        private const int ErrorValue = -1;
        public int CurrentItemNum { get; private set; }
        public Action OnItemNumChanged;
        public Action<Sprite> OnItemSpriteChanged;
        public Action<int> OnItemCountChanged;
        public Action<int> OnKeyCountChanged;
        public Action<ItemPrefabSet[]> OnItemLineupChanged;
        public bool CurrentIsUse => CurrentItemNum != ErrorValue && _itemSets[CurrentItemNum]._isUse;
        public GameObject CurrentPredict => CurrentItemNum == ErrorValue ? null : _itemSets[CurrentItemNum]._currentPrefab;
        private LogTextHandler _logTextHandler;
        private const string LogTemplate = "を手に入れた。";
        private const string LogObeliskTemplate = "破片は集まった。オベリスクへ向かおう。";
        
        private void Awake()
        {
            CheckSingleton();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
                _itemSets[i]._currentPrefab = Instantiate(_itemSets[i]._predict);
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
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F)) IncreaseItemNum();
        }

        private void IncreaseItemNum()
        {
            var startIndex = Math.Max(CurrentItemNum, 0);

            for (var i = 1; i <= _itemSets.Length; i++)
            {
                var newIndex = (startIndex + i) % _itemSets.Length;

                if (_itemSets[newIndex]._count <= 0) continue;
                ChangeItemNum(newIndex);
                return;
            }

            ChangeItemNum(ErrorValue);
        }

        private void ChangeItemNum(int value)
        {
            if (CurrentItemNum != ErrorValue)
            {
                ChangePredictActive(_itemSets[CurrentItemNum]._currentPrefab, false);
            }
            CurrentItemNum = value;
            var sprite = CurrentItemNum != ErrorValue ? _itemSets[value]._sprite : null;
            OnItemSpriteChanged?.Invoke(sprite);
            OnItemNumChanged?.Invoke();
            ChangeItemCount();
        }

        private void ChangeItemCount()
        {
            if (CurrentItemNum == ErrorValue )
            {
                OnItemCountChanged?.Invoke(0);
                return;
            }
            OnItemCountChanged?.Invoke(_itemSets[CurrentItemNum]._count);
            if (_itemSets[CurrentItemNum]._kind == ItemKind.Key) 
                OnKeyCountChanged?.Invoke(_itemSets[CurrentItemNum]._count);
        }

        // アイテムをインベントリに追加する
        public void AddItem(ItemKind item)
        {
            for (var i = 0; i < _itemSets.Length; i++)
            {
                if (_itemSets[i]._kind != item) continue;
                _itemSets[i]._count++;
                var message = _itemSets[i]._name + LogTemplate;
                _logTextHandler.AddLog(message);
                if (item == ItemKind.Key) CheckObeliskCount();
                if (CurrentItemNum == i) ChangeItemCount();
                if (_itemSets[i]._count == 1) OnItemLineupChanged(_itemSets);
                if (CurrentItemNum == ErrorValue) ChangeItemNum(i);
                break;
            }
        }

        private void CheckObeliskCount()
        {
            if (_itemSets[(int)ItemKind.Key]._count == ExitObelisk.NeededKeyCount)
            {
                _logTextHandler.AddLog(LogObeliskTemplate);
            }
        }

        public GameObject UseItem()
        {
            if (CurrentItemNum == ErrorValue) return null;
            var outItem = _itemSets[CurrentItemNum]._prefab;
            _itemSets[CurrentItemNum]._count = Math.Max(0, _itemSets[CurrentItemNum]._count - 1);
            ChangeItemCount();

            if (_itemSets[CurrentItemNum]._count > 0) return outItem;
            OnItemLineupChanged(_itemSets);
            ChangeItemNum(ErrorValue);
            return outItem;
        }

        public void ChangePredictActive(GameObject predict, bool isVisible)
        {
            if (predict == null) return;
            predict.SetActive(isVisible);
        }
        
    }
}