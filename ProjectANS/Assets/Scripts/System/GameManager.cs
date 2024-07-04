using System.Map;
using Cinemachine;
using UnityEngine;

namespace System
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _partnerPrefab;
        [SerializeField] private StageGenerator _stageGenerator;
        private Vector3 _playerInsPos = Vector3.zero;
        private Vector3 _partnerInsPos = Vector3.zero;
        [SerializeField] private CinemachineVirtualCamera _vCam;
        [SerializeField] private NavMeshBaker _navMeshBaker;
        private void Awake()
        {
            _stageGenerator.MapGenerate();
            _navMeshBaker.BakeNavMesh();
            _playerInsPos.x = _stageGenerator.RoomInfo[0, (int) StageGenerator.RoomStatus.CenterX];
            _playerInsPos.y = 5;
            _playerInsPos.z = _stageGenerator.RoomInfo[0, (int) StageGenerator.RoomStatus.CenterZ];
            var player = Instantiate(_playerPrefab, _playerInsPos, Quaternion.identity);
            _vCam.Follow = player.transform;
            
            _partnerInsPos.x = _stageGenerator.RoomInfo[0, (int) StageGenerator.RoomStatus.CenterX] - (_partnerPrefab.transform.localScale.x + _playerPrefab.transform.localScale.x);
            _partnerInsPos.y = 5;
            _partnerInsPos.z = _stageGenerator.RoomInfo[0, (int) StageGenerator.RoomStatus.CenterZ];
            Instantiate(_partnerPrefab, _partnerInsPos, Quaternion.identity);
        }
    }
}
