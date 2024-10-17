using System;
using UnityEngine;

namespace Character.Player
{
    public class PlayerHpHandler : MonoBehaviour
    {
        [SerializeField] private int _maxHp;
        public int MaxHp => _maxHp;
        public int CurrentHp { get; private set; }
        public Action<int> OnChangeHp;

        private void Awake()
        {
            CurrentHp = _maxHp;
        }

        public void ReceiveDamage(int damage)
        {
            CurrentHp = Math.Max(CurrentHp - damage, 0);
            OnChangeHp?.Invoke(CurrentHp);
        }
    }
}
