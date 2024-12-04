using System;
using Manager;
using UnityEngine;

namespace Character.Player
{
    public class PlayerHpHandler : MonoBehaviour
    {
        [SerializeField] private PlayerAnimationCnt _playerAnimationCnt;
        public Action OnDie;
        public bool IsDie { get; private set; }
        private DungeonLayerHandler _dungeonLayerHandler;
        private PlayerStatusHandler _playerStatusHandler;
        
        private void Start()
        {
            _dungeonLayerHandler = GameObject.FindWithTag("DungeonLayerHandler").GetComponent<DungeonLayerHandler>();
            _playerStatusHandler = GameObject.FindWithTag("PlayerStatusHandler").GetComponent<PlayerStatusHandler>();
            OnDie += _playerAnimationCnt.SetIsDie;
            OnDie += _dungeonLayerHandler.PlayerDeathNext;
        }
        
        private void OnDestroy()
        {
            OnDie -= _playerAnimationCnt.SetIsDie;
            OnDie -= _dungeonLayerHandler.PlayerDeathNext;
        }

        public void ReceiveDamage(int damage)
        {
            if (IsDie) return;
            var newHp = Math.Max(_playerStatusHandler.PlayerCurrentHp - damage, 0);
            newHp = Math.Min(newHp, _playerStatusHandler.MaxHp);
            _playerStatusHandler.SetPlayerCurrentHp(newHp);
            if (newHp > 0) return;
            IsDie = true;
            OnDie?.Invoke();
        }
    }
}
