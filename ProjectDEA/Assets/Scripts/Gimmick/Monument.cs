using System;
using Item;
using Manager;
using Manager.MetaAI;
using UI;
using UnityEngine;

namespace Gimmick
{
    public class Monument : MonoBehaviour, IInteractable, IGimmickID
    {
        private LogTextHandler _logTextHandler;
        private const ItemKind OutItem = ItemKind.PowerApple;
        private InventoryHandler _inventoryHandler;
        public event Action Destroyed;
        public bool IsInteractable { get; private set; }
        private const string AddLogMessage = "崩れた壁の中から墓石が現れた。";
        private MetaAIHandler _metaAIHandler;
        [SerializeField] private MetaAIHandler.AddScores[] _findScores;
        public GimmickID GimmickIdInfo { get; set; }
        public event Action<IGimmickID> Returned;
        
        private void Start()
        {
            _logTextHandler = GameObject.FindWithTag("LogTextHandler").GetComponent<LogTextHandler>();
            _logTextHandler.AddLog(AddLogMessage);
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            IsInteractable = true;
            _metaAIHandler = GameObject.FindWithTag("MetaAI").GetComponent<MetaAIHandler>();
        }

        public void Interact()
        {
            _inventoryHandler.AddItem(OutItem);
            IsInteractable = false;
            _metaAIHandler.SendLogsForMetaAI(_findScores);
        }
    }
}
