using Gimmick;
using Manager;
using UnityEngine;

namespace Mission
{
    public class DebugMission : MonoBehaviour
    {
        private MissionStateHandler _missionStateHandler;

        private void Start()
        {
            var gameEventManager = GameObject.FindWithTag("GameEventManager").GetComponent<GameEventManager>();
            var roomGimmickGenerator = GameObject.FindWithTag("GimmickGenerator").GetComponent<RoomGimmickGenerator>();
            var inventoryHandler = GameObject.FindWithTag("InventoryHandler").GetComponent<InventoryHandler>();
            _missionStateHandler = new MissionStateHandler(gameEventManager, roomGimmickGenerator, inventoryHandler);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DoMission();
            }
        }
        
        public void DoMission()
        {
            if (_missionStateHandler.DoingMission) return;
            _missionStateHandler.StartMission();
        }
    }
}
