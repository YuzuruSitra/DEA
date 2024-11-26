using Manager.Map;
using UnityEngine;

namespace Gimmick
{
    public class RoomGimmickHandler : MonoBehaviour
    {
        [SerializeField] private RoomGimmickGenerator _roomGimmickGenerator;
        [SerializeField] private Transform _player;
        private InRoomChecker _roomChecker;
        private int _currentPlayerRoom = -1;
        
        private void Start()
        {
            _roomChecker = new InRoomChecker();
        }
        
        private void Update()
        {
            var playerRoom = _roomChecker.CheckStayRoomNum(_player.position);
            if (_currentPlayerRoom == playerRoom) return;
            _currentPlayerRoom = playerRoom;
            _roomGimmickGenerator.RandomGenerateGimmicks(playerRoom);
        }
    }
}
