using Manager.Map;
using UnityEngine;

namespace Gimmick
{
    public class RoomGimmickHandler : MonoBehaviour
    {
        [SerializeField] private RoomGimmickGenerator _roomGimmickGenerator;
        private PlayerRoomTracker _playerRoomTracker;
        
        private void Start()
        {
            OnGenerateGimmick();
        }

        private void OnGenerateGimmick()
        {
            var playerRoom = _playerRoomTracker.CurrentPlayerRoom;
            _roomGimmickGenerator.RandomGenerateGimmicks(playerRoom);
        }
    }
}
