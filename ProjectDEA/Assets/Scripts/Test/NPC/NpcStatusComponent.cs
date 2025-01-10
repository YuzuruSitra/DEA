using UnityEngine;

namespace Test.NPC
{
    public class NpcStatusComponent : MonoBehaviour
    {
        [SerializeField] private float _maxStamina;
        public float CurrentStamina { get; private set; }
        
        [SerializeField] private float _maxFullness;
        public float CurrentFullness { get; private set; }
        
        private void Start()
        {
            CurrentStamina = _maxStamina;
            CurrentFullness = _maxFullness;
        }

        private void ConsumeStamina(float amount)
        {

        }

        private void RecoverStamina(float amount)
        {

        }

	
        public void GainStamina(float amount)
        {
            CurrentStamina += amount;
            CurrentStamina = Mathf.Clamp(CurrentStamina, 0, _maxStamina);
        }
	
        public void ChangeFullness(float amount)
        {
            CurrentFullness += amount;
            CurrentFullness = Mathf.Clamp(CurrentFullness, 0, _maxFullness);
        }
    }
}
