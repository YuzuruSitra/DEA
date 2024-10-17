using Manager.Map;
using UnityEngine;

namespace Gimmick
{
    public class EnemySpawnArea : MonoBehaviour
    {
        private Transform _player;
        [SerializeField] private GameObject[] _insEnemyList;
        private InRoomChecker _roomChecker;
        private GameObject _currentEnemy;
        private int _spawnRoom;

        private void Start()
        {
            _player = GameObject.FindWithTag("Player").transform;
            _roomChecker = new InRoomChecker();
            _spawnRoom = _roomChecker.CheckStayRoomNum(transform.position);
            var rnd = Random.Range(0, _insEnemyList.Length);
            _currentEnemy = Instantiate(_insEnemyList[rnd], transform.position, Quaternion.identity);
        }

        private void Update()
        {
            var playerRoomNum = _roomChecker.CheckStayRoomNum(_player.position);
            if (_currentEnemy.activeSelf) return;
            if (playerRoomNum != _spawnRoom) _currentEnemy.SetActive(true);
        }
    }
}
