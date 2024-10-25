using UnityEngine;

namespace Character.Player
{
    public class PlayerClasHub : MonoBehaviour
    {
        [SerializeField] private PlayerMover _playerMover;
        public PlayerMover PlayerMover => _playerMover;
        [SerializeField] private PlayerHpHandler _playerHpHandler;
        public PlayerHpHandler PlayerHpHandler => _playerHpHandler;
        [SerializeField] private PlayerInteraction _playerInteraction;
        [SerializeField] private PlayerAttackHandler _playerAttackHandler;
        public PlayerAttackHandler PlayerAttackHandler => _playerAttackHandler;
        [SerializeField] private PlayerUseItem _playerUseItem;
        
        public void SetPlayerFreedom(bool isFreedom)
        {
            _playerMover.SetWalkableState(isFreedom);
            _playerInteraction.SetInteractableState(isFreedom);
            _playerAttackHandler.SetCanAttackState(isFreedom);
            _playerUseItem.SetCanUseItemState(isFreedom);
        }
    }
}
