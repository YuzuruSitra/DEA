using Character.NPC.EnemyDragon;
using Manager.Map;
using UnityEngine;

namespace Gimmick
{
    public class DragonSpawnArea : MonoBehaviour
    {
        private Transform _player;
        [SerializeField] private DragonController _targetDragon;
        private InRoomChecker _roomChecker;
        private GameObject _currentEnemy;
        private int _spawnRoom;

        private void Start()
        {
            _player = GameObject.FindWithTag("Player").transform;
            _roomChecker = new InRoomChecker();
            _spawnRoom = _roomChecker.CheckStayRoomNum(transform.position);
            _currentEnemy = Instantiate(_targetDragon.gameObject, transform.position, Quaternion.identity);
        }

        private void Update()
        {
            var playerRoomNum = _roomChecker.CheckStayRoomNum(_player.position);
            if (_currentEnemy.activeSelf || playerRoomNum == InRoomChecker.ErrorRoomNum) return;
            if (playerRoomNum != _spawnRoom) _currentEnemy.SetActive(true);
        }
    }
}
