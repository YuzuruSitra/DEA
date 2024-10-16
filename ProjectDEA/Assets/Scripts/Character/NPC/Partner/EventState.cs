using UnityEngine;

namespace Character.NPC.Partner
{
    public class EventState : INpcAiState
    {
        private const float StateTime = 5.0f;
        private float _remainTime;
        // 仮
        public bool IsStateFin => (_remainTime <= 0);
        
        public void EnterState()
        {
            _remainTime = StateTime;
        }

        public void UpdateState()
        {
            _remainTime -= Time.deltaTime;
        }

        public void ExitState()
        {

        }
    }
}
