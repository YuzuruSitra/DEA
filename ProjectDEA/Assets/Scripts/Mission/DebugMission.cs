using UnityEngine;

namespace Mission
{
    public class DebugMission : MonoBehaviour
    {
        private MissionStateHandler _missionStateHandler;

        private void Start()
        {
            var gameEventManager = GameObject.FindWithTag("GameEventManager").GetComponent<GameEventManager>();
            _missionStateHandler = new MissionStateHandler(gameEventManager);
        }

        public void DoMission()
        {
            _missionStateHandler.StartMission();
        }
    }
}
