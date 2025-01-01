using UnityEngine;

namespace Test.NPC
{
	public class HealthComponent : MonoBehaviour
	{
		public float _maxHealth;
		public float CurrentHealth { get; private set; }

		public delegate void HealthChanged(float currentHealth, float maxHealth);
		public event HealthChanged OnHealthChanged;

		public delegate void Died();
		public event Died OnDeath;

		private void Start()
		{
			CurrentHealth = _maxHealth;
		}
	
		public void TakeDamage(float amount)
		{
			if (amount <= 0) return;

			CurrentHealth -= amount;
			CurrentHealth = Mathf.Clamp(CurrentHealth, 0, _maxHealth);

			OnHealthChanged?.Invoke(CurrentHealth, _maxHealth);

			if (CurrentHealth <= 0)
			{
				Die();
			}
		}
	
		public void Heal(float amount)
		{
			if (amount <= 0) return;

			CurrentHealth += amount;
			CurrentHealth = Mathf.Clamp(CurrentHealth, 0, _maxHealth);

			OnHealthChanged?.Invoke(CurrentHealth, _maxHealth);
		}
	
		private void Die()
		{
			//Debug.Log($"{gameObject.name} has died.");
			OnDeath?.Invoke();
			
			gameObject.SetActive(false);
		}
	}
}