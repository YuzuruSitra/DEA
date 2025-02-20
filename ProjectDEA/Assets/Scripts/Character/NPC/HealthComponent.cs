using System;
using UnityEngine;

namespace Character.NPC
{
	public class HealthComponent
	{
		public float MaxHealth { get; }
		public float CurrentHealth { get; private set; }
		public event Action<float> OnHealthChanged;
		public event Action OnDeath;

		public HealthComponent(float initialHealth)
		{
			MaxHealth = initialHealth;
			CurrentHealth = initialHealth;
		}
	
		public void TakeDamage(float amount)
		{
			CurrentHealth -= amount;
			CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

			OnHealthChanged?.Invoke(CurrentHealth);

			if (CurrentHealth <= 0)
			{
				OnDeath?.Invoke();
			}
		}
	
		public void Heal(float amount)
		{
			if (amount <= 0) return;

			CurrentHealth += amount;
			CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

			OnHealthChanged?.Invoke(CurrentHealth);
		}
	}
}