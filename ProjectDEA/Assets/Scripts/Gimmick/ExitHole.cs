using System;
using System.Collections;
using Cinemachine;
using Item;
using Manager;
using UnityEngine;

namespace Gimmick
{
    public class ExitHole : MonoBehaviour, IInteractable
    {
        private DungeonLayerHandler _dungeonLayerHandler;
        private InventoryHandler _inventoryHandler;

        [SerializeField] private int _neededKeyCount;
        [SerializeField] private CinemachineVirtualCameraBase _vCam;
        private const int Priority = 15;
        public event Action Destroyed;
        
        private void Start()
        {
            _dungeonLayerHandler = GameObject.FindWithTag("DungeonLayerHandler").GetComponent<DungeonLayerHandler>();
            _inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
        }
        
        public void Interact()
        {
            if (OnCertification())
            {
                _vCam.Priority = Priority;
                StartCoroutine(ExitLayer());
            }
            else
            {
                Debug.Log("Authentication Failed");
            }
        }

        private bool OnCertification()
        {
            return _neededKeyCount <= _inventoryHandler.ItemSets[(int)ItemKind.Key]._count;
        }

        private IEnumerator ExitLayer()
        {
            yield return new WaitForSeconds(3.0f);
            _dungeonLayerHandler.NextDungeonLayer();
        }
        
    }
}
