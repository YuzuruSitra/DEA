using Manager;
using TMPro;
using UnityEngine;

namespace UI
{
    public class MemoirsUIHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _memoirsContent;
        [SerializeField] private GameObject _contentPrefab;
        private InventoryHandler _inventoryHandler;
        
        private GameObject[] _memoirsIndex;
        
        private void Start()
        {
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            _memoirsIndex = new GameObject[_inventoryHandler.MemoirsDataSet.Length];
            for (var i = 0; i < _inventoryHandler.MemoirsDataSet.Length; i++)
            {
                _memoirsIndex[i] = Instantiate(_contentPrefab, _memoirsContent.transform);
                var cd1 = _memoirsIndex[i].transform.GetChild(0).gameObject;
                var nameText = cd1.GetComponent<TextMeshProUGUI>();
                nameText.text = _inventoryHandler.MemoirsDataSet[i]._title;
                
                var cd2 = _memoirsIndex[i].transform.GetChild(1).gameObject;
                var contentTxt = cd2.GetComponent<TextMeshProUGUI>();
                contentTxt.text = _inventoryHandler.MemoirsDataSet[i]._content;
            }

            _inventoryHandler.OnMemoirsChanged += ChangeMemoirsPanel;
        }

        private void OnDestroy()
        {
            _inventoryHandler.OnMemoirsChanged-= ChangeMemoirsPanel;
        }

        private void ChangeMemoirsPanel()
        {
            for (var i = 0; i < _inventoryHandler.MemoirsDataSet.Length; i++)
            {
                _memoirsIndex[i].SetActive(_inventoryHandler.MemoirsDataSet[i]._active);
            }
        }
    }
}
