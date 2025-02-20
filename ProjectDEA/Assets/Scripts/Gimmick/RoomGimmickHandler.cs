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
            _playerRoomTracker = GameObject.FindWithTag("PlayerRoomTracker").GetComponent<PlayerRoomTracker>();
            OnGenerateGimmick();
        }

        private void OnGenerateGimmick()
        {
            var playerRoom = _playerRoomTracker.CurrentPlayerRoom;
            _roomGimmickGenerator.RandomGenerateGimmicks(playerRoom);
        }
    }
}
