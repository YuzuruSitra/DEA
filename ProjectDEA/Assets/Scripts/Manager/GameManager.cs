using Manager.Map;
using UnityEngine;
using Gimmick;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject _player;
        [SerializeField] private StageGenerator _stageGenerator;
        [SerializeField] private GimmickGenerator _gimmickGenerator;
        private Vector3 _playerSetPos = Vector3.zero;
        [SerializeField] private NavMeshBaker _navMeshBaker;
        private void Awake()
        {
            _stageGenerator.MapGenerate();
            _gimmickGenerator.GenerateGimmick(_stageGenerator);
            _navMeshBaker.BakeNavMesh();
            _playerSetPos.x = _stageGenerator.RoomInfo[0, (int) StageGenerator.RoomStatus.CenterX];
            _playerSetPos.y = 1.88f;
            _playerSetPos.z = _stageGenerator.RoomInfo[0, (int) StageGenerator.RoomStatus.CenterZ];
            _player.transform.position = _playerSetPos;
        }
    }
}
