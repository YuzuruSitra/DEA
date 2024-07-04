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
        private Vector3 _playerInsPos;
        private Vector3 _partnerInsPos;
        [SerializeField] private CinemachineVirtualCamera _vCam;
        private void Awake()
        {
            _stageGenerator.MapGenerate();
            _playerInsPos = _stageGenerator.GetRoomCenters(0);
            var player = Instantiate(_playerPrefab, _playerInsPos, Quaternion.identity);
            _vCam.Follow = player.transform;
            
            _partnerInsPos = _stageGenerator.GetRoomCenters(0);
            _partnerInsPos.x -= _partnerPrefab.transform.localScale.x + _playerPrefab.transform.localScale.x;
            Instantiate(_partnerPrefab, _partnerInsPos, Quaternion.identity);
        }
    }
}
