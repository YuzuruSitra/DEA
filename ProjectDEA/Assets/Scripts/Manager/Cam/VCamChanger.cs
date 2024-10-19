using System;
using Cinemachine;
using Manager.Map;
using UnityEngine;

namespace Manager.Cam
{
    public class VCamChanger : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCameraBase _roomVCam;
        [SerializeField] private CinemachineVirtualCameraBase _roadVCam;
        [SerializeField] private Transform _player;
        private InRoomChecker _roomChecker;
        private const int LowPriority = 5;
        private const int HighPriority = 10;

        private void Start()
        {
            _roomChecker = new InRoomChecker();
        }

        private void Update()
        {
            SetCameraPriority(_roomChecker.CheckStayRoomNum(_player.position));
        }

        private void SetCameraPriority(int playerRoom)
        {
            var isInRoom = playerRoom != InRoomChecker.ErrorRoomNum;
            _roomVCam.Priority = isInRoom ? HighPriority : LowPriority;
            _roadVCam.Priority = isInRoom ? LowPriority : HighPriority;
        }
    }
}