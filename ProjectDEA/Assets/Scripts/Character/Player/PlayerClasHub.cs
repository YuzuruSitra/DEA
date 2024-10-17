using UnityEngine;

namespace Character.Player
{
    public class PlayerClasHub : MonoBehaviour
    {
        [SerializeField] private PlayerMover _playerMover;
        public PlayerMover PlayerMover => _playerMover;
        [SerializeField] private PlayerHpHandler _playerHpHandler;
        public PlayerHpHandler PlayerHpHandler => _playerHpHandler;
    }
}
