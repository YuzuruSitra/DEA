using UnityEngine;

namespace Test.NPC
{
    public class NpcStatusComponent : MonoBehaviour
    {
        private const int CalcTiming = 1;
        
        [SerializeField] private float _staminaChangeSecond;
        [SerializeField] private float _maxStamina;
        public float CurrentStamina { get; private set; }
        private float _staminaConsumeTimer;
        private float _staminaRecoverTimer;
        
        [SerializeField] private float _fullnessChangeSecond;
        [SerializeField] private float _maxFullness;
        public float CurrentFullness { get; private set; }
        private float _fullnessConsumeTimer;
        
        private void Start()
        {
            CurrentStamina = _maxStamina;
            CurrentFullness = _maxFullness;
        }

        public void ConsumeStamina()
        {
            _staminaConsumeTimer += Time.deltaTime;
            if (!(_staminaConsumeTimer >= CalcTiming)) return;
            CurrentStamina -= _staminaChangeSecond;
            CurrentStamina = Mathf.Clamp(CurrentStamina, 0, _maxStamina);
            _staminaConsumeTimer = 0f;
        }

        public void RecoverStamina()
        {
            _staminaRecoverTimer += Time.deltaTime;
            if (!(_staminaRecoverTimer >= CalcTiming)) return;
            CurrentStamina += _staminaChangeSecond;
            CurrentStamina = Mathf.Clamp(CurrentStamina, 0, _maxStamina);
            _staminaRecoverTimer = 0f;
        }
        
        public void ConsumeFullness()
        {
            _fullnessConsumeTimer += Time.deltaTime;
            if (!(_fullnessConsumeTimer >= CalcTiming)) return;
            CurrentFullness -= _fullnessChangeSecond;
            CurrentFullness = Mathf.Clamp(CurrentFullness, 0, _maxFullness);
            _fullnessConsumeTimer = 0f;
        }
        
        public void AddFullness(float amount)
        {
            CurrentFullness += amount;
            CurrentFullness = Mathf.Clamp(CurrentFullness, 0, _maxFullness);
        }
    }
}
