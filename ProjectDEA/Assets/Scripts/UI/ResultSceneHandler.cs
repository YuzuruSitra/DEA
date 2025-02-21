using Manager;
using Manager.MetaAI;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ResultSceneHandler : MonoBehaviour
    {
        [SerializeField] private string[] _resultString;
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private GameObject _clearObj;
        [SerializeField] private GameObject _failedObj;
        [SerializeField] private TextMeshProUGUI _playerType;
        
        private void Start()
        {
            var dungeonLayerHandler = GameObject.FindWithTag("DungeonLayerHandler").GetComponent<DungeonLayerHandler>();
            _resultText.text = dungeonLayerHandler.IsGameClear ? _resultString[0] : _resultString[1];
            var target = dungeonLayerHandler.IsGameClear ? _clearObj : _failedObj;
            target.SetActive(true);
            var inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            var metaAIHandler = GameObject.FindWithTag("MetaAI").GetComponent<MetaAIHandler>();
            _playerType.text = metaAIHandler.CurrentPlayerType.ToString();
        }
    }
}
