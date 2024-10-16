using Character.NPC.EnemyDragon;
using UnityEngine;

namespace Character.NPC
{
    public class EventState : INpcAiState
    {
        public DragonAnimCtrl.AnimState CurrentAnim { get; private set; }
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
