using System.Map;
using UnityEngine;

namespace System
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject _partner;
        [SerializeField] private StageGenerator _stageGenerator;
        private Vector3 _playerSetPos = Vector3.zero;
        private Vector3 _partnerSetPos = Vector3.zero;
        [SerializeField] private NavMeshBaker _navMeshBaker;
        private void Awake()
        {
            _stageGenerator.MapGenerate();
            _navMeshBaker.BakeNavMesh();
            _playerSetPos.x = _stageGenerator.RoomInfo[0, (int) StageGenerator.RoomStatus.CenterX];
            _playerSetPos.y = 1.88f;
            _playerSetPos.z = _stageGenerator.RoomInfo[0, (int) StageGenerator.RoomStatus.CenterZ];
            _player.transform.position = _playerSetPos;
            
            _partnerSetPos.x = _stageGenerator.RoomInfo[0, (int) StageGenerator.RoomStatus.CenterX] - (_partner.transform.localScale.x + _player.transform.localScale.x);
            _partnerSetPos.y = 1.88f;
            _partnerSetPos.z = _stageGenerator.RoomInfo[0, (int) StageGenerator.RoomStatus.CenterZ];
            _partner.transform.position = _partnerSetPos;
            _partner.SetActive(true);
        }
    }
}
