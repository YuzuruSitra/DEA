using System;
using System.Collections;
using Cinemachine;
using Item;
using Manager;
using UnityEngine;

namespace Gimmick
{
    public class ExitObelisk : MonoBehaviour, IInteractable
    {
        private DungeonLayerHandler _dungeonLayerHandler;
        private InventoryHandler _inventoryHandler;

        private const int NeededKeyCount = 4;
        private int _setKeyCount;
        [SerializeField] private CinemachineVirtualCameraBase _vCam;
        [SerializeField] private GameObject[] _obeliskSids;
        private const int Priority = 15;
        public event Action Destroyed;
        
        private void Start()
        {
            _dungeonLayerHandler = GameObject.FindWithTag("DungeonLayerHandler").GetComponent<DungeonLayerHandler>();
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
        }
        
        public void Interact()
        {
            if (_setKeyCount >= NeededKeyCount)
            {
                _vCam.Priority = Priority;
                StartCoroutine(ExitLayer());
                return;
            }

            if (_inventoryHandler.CurrentItemNum != (int)ItemKind.Key) return;
            _inventoryHandler.UseItem();
            SetKey();
        }

        private void SetKey()
        {
            if (_setKeyCount < _obeliskSids.Length) _obeliskSids[_setKeyCount].SetActive(true);
            _setKeyCount++;
        }

        private IEnumerator ExitLayer()
        {
            yield return new WaitForSeconds(3.0f);
            _dungeonLayerHandler.NextDungeonLayer();
        }
        
    }
}