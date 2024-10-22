using System;
using UnityEngine;

namespace Character.Player
{
    public class PlayerHpHandler : MonoBehaviour
    {
        [SerializeField] private PlayerAnimationCnt _playerAnimationCnt;
        [SerializeField] private int _maxHp;
        public int MaxHp => _maxHp;
        public int CurrentHp { get; private set; }
        public Action<int> OnChangeHp;
        private Action _onDie;
        public bool IsDie { get; private set; }

        private void Awake()
        {
            CurrentHp = _maxHp;
        }

        private void Start()
        {
            _onDie += _playerAnimationCnt.SetIsDie;
        }
        
        private void OnDestroy()
        {
            _onDie -= _playerAnimationCnt.SetIsDie;
        }

        public void ReceiveDamage(int damage)
        {
            if (IsDie) return;
            CurrentHp = Math.Max(CurrentHp - damage, 0);
            if (CurrentHp <= 0)
            {
                IsDie = true;
                _onDie?.Invoke();
            }
            OnChangeHp?.Invoke(CurrentHp);
        }
    }
}
