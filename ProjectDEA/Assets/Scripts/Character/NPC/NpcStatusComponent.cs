using UnityEngine;

namespace Character.NPC
{
    public class NpcStatusComponent
    {
        private const int CalcTiming = 1;
        
        private readonly float _staminaChangeSecond;
        private readonly float _fullnessChangeSecond;
        public const float MaxStamina = 100f;
        public float CurrentStamina { get; private set; }
        private float _staminaConsumeTimer;
        private float _staminaRecoverTimer;
        public const float MaxFullness = 100f;
        public float CurrentFullness { get; private set; }
        private float _fullnessConsumeTimer;

        public NpcStatusComponent(float staminaChangeSecond, float fullnessChangeSecond)
        {
            _staminaChangeSecond = staminaChangeSecond;
            _fullnessChangeSecond = fullnessChangeSecond;
            CurrentStamina = Random.Range(0, MaxStamina);
            CurrentFullness = Random.Range(0, MaxFullness);
        }

        public void ConsumeStamina()
        {
            _staminaConsumeTimer += Time.deltaTime;
            if (!(_staminaConsumeTimer >= CalcTiming)) return;
            CurrentStamina -= _staminaChangeSecond;
            CurrentStamina = Mathf.Clamp(CurrentStamina, 0, MaxStamina);
            _staminaConsumeTimer = 0f;
        }

        public void RecoverStamina()
        {
            _staminaRecoverTimer += Time.deltaTime;
            if (!(_staminaRecoverTimer >= CalcTiming)) return;
            CurrentStamina += _staminaChangeSecond;
            CurrentStamina = Mathf.Clamp(CurrentStamina, 0, MaxStamina);
            _staminaRecoverTimer = 0f;
        }
        
        public void ConsumeFullness()
        {
            _fullnessConsumeTimer += Time.deltaTime;
            if (!(_fullnessConsumeTimer >= CalcTiming)) return;
            CurrentFullness -= _fullnessChangeSecond;
            CurrentFullness = Mathf.Clamp(CurrentFullness, 0, MaxFullness);
            _fullnessConsumeTimer = 0f;
        }
        
        public void AddFullness(float amount)
        {
            CurrentFullness += amount;
            CurrentFullness = Mathf.Clamp(CurrentFullness, 0, MaxFullness);
        }
    }
}
