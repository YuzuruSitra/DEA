using System;
using Item;
using Manager;
using Manager.MetaAI;
using UI;
using UnityEngine;

namespace Gimmick
{
    public class RaggedMemoirs : MonoBehaviour, IInteractable, IGimmickID
    {
        public event Action Destroyed;
        public event Action<IGimmickID> Returned;
        public bool IsInteractable { get; private set; }
        public GimmickID GimmickIdInfo { get; set; }
        private InventoryHandler _inventoryHandler;
        
        private LogTextHandler _logTextHandler;
        private readonly string[] _addLogMessage =
        {
            "ぼろぼろな手記だが、かろうじて読めるページもある。",
            "It is a tattered memoir, but some pages are barely readable."
        };
        private MetaAIHandler _metaAIHandler;
        [SerializeField] private MetaAIHandler.AddScores[] _findScores;
        
        private void Start()
        {
            _logTextHandler = GameObject.FindWithTag("LogTextHandler").GetComponent<LogTextHandler>();
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            IsInteractable = true;
            _metaAIHandler = GameObject.FindWithTag("MetaAI").GetComponent<MetaAIHandler>();
        }
        
        public void Interact()
        {
            _logTextHandler.AddLog(_addLogMessage[(int)_logTextHandler.LanguageHandler.CurrentLanguage]);
            _metaAIHandler.SendLogsForMetaAI(_findScores);
            _inventoryHandler.AddItem(ItemKind.RaggedMemoirs);
            
            IsInteractable = false;
        }
    }
}
