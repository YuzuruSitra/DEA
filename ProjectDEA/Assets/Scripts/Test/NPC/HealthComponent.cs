using System;
using UnityEngine;

namespace Test.NPC
{
	public class HealthComponent : MonoBehaviour
	{
		[SerializeField] private float _initialHealth;
		public float MaxHealth { get; private set; }
		public float CurrentHealth { get; private set; }

		public delegate void HealthChanged(float currentHealth, float maxHealth);
		public event HealthChanged OnHealthChanged;
		public event Action OnDeath;

		private void Awake()
		{
			MaxHealth = _initialHealth;
			CurrentHealth = _initialHealth;
		}
	
		public void TakeDamage(float amount)
		{
			if (amount <= 0) return;

			CurrentHealth -= amount;
			CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

			OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

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

			OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
		}
	}
}