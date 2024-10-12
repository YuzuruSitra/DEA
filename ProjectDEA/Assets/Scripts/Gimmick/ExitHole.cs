using System.Collections;
using Cinemachine;
using Manager;
using UnityEngine;

namespace Gimmick
{
    public class ExitHole : MonoBehaviour, IInteractable
    {
        private DungeonLayerHandler _dungeonLayerHandler;
        [SerializeField] private CinemachineVirtualCameraBase _vCam;
        private const int Priority = 15;
        
        private void Start()
        {
            _dungeonLayerHandler = GameObject.FindWithTag("DungeonLayerHandler").GetComponent<DungeonLayerHandler>();
        }
        
        public void Interact()
        {
            _vCam.Priority = Priority;
            StartCoroutine(ExitLayer());
        }

        private IEnumerator ExitLayer()
        {
            yield return new WaitForSeconds(3.0f);
            _dungeonLayerHandler.NextDungeonLayer();
        }
        
    }
}
