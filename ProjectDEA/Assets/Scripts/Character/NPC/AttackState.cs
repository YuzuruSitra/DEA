
namespace Character.NPC
{
    public class AttackState : INpcAiState
    {
        private float _remainTime;
        public bool IsStateFin => (_remainTime <= 0);
        
        public void EnterState()
        {
            
        }

        public void UpdateState()
        {
            
        }

        public void ExitState()
        {

        }
    }
}
