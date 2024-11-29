using System;
using UnityEngine;

namespace Manager.Map
{
    public class PlayerRoomTracker : MonoBehaviour
    {
        [SerializeField] private Transform _player;
        private InRoomChecker _roomChecker;
        private int _beforePlayerRoom;
        public int CurrentPlayerRoom { get; private set; }
        public event Action OnPlayerRoomChange;
        private void Start()
        {
            _roomChecker = new InRoomChecker();
        }

        private void Update()
        {
            CurrentPlayerRoom = _roomChecker.CheckStayRoomNum(_player.position);
            if (_beforePlayerRoom == CurrentPlayerRoom || CurrentPlayerRoom == InRoomChecker.ErrorRoomNum) return;
            _beforePlayerRoom = CurrentPlayerRoom;
            OnPlayerRoomChange?.Invoke();
        }
    }
}