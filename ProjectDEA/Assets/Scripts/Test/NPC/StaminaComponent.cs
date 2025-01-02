using UnityEngine;

namespace Test.NPC
{
    public class StaminaComponent : MonoBehaviour
    {
        public float _maxStamina;
        public float CurrentStamina { get; private set; }

        private void Start()
        {
            CurrentStamina = _maxStamina;
        }
	
        public void DecreaseStamina(float amount)
        {
            if (amount <= 0) return;

            CurrentStamina -= amount;
            CurrentStamina = Mathf.Clamp(CurrentStamina, 0, _maxStamina);
        }
	
        public void IncreaseStamina(float amount)
        {
            if (amount <= 0) return;

            CurrentStamina += amount;
            CurrentStamina = Mathf.Clamp(CurrentStamina, 0, _maxStamina);
        }
    }
}
