using System;
using Manager.Map;
using Mission;
using UnityEngine;
using Random = System.Random;

namespace Gimmick
{
    public class MoveGhost : MonoBehaviour, IInteractable, IGimmickID
    {
        [SerializeField] private int _gimmickID;
        private InRoomChecker _roomChecker;
        private StageGenerator _stageGenerator;
        private GameEventManager _gameEventManager;
        public event Action<IGimmickID> Returned;
        public GimmickID GimmickIdInfo { get; set; }
        public event Action Destroyed;
        public bool IsInteractable => !_isMoving;

        [SerializeField] private int _roomChangeCount;
        private int _currentChangeCount;
        private bool _isMoving;
        private Vector3 _targetPos;
        [SerializeField] private float _speed;
        private Quaternion _initialQuaternion;
        
        private void Start()
        {
            _stageGenerator = GameObject.FindWithTag("StageGenerator").GetComponent<StageGenerator>();
            _gameEventManager = GameObject.FindWithTag("GameEventManager").GetComponent<GameEventManager>();
            _roomChecker = new InRoomChecker();
            _initialQuaternion = transform.rotation;
        }

        private void Update()
        {
            if (!_isMoving) return;
            // ゴーストを _targetPos へ移動
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, _speed * Time.deltaTime);

            // 進行方向に向けて回転
            var direction = (_targetPos - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _speed * Time.deltaTime);
            }

            // 目的地に到達したかチェック
            if (!(Vector3.Distance(transform.position, _targetPos) < 0.1f)) return;
            _isMoving = false;
            transform.rotation = _initialQuaternion;
        }

        public void Interact()
        {
            if (!IsInteractable) return;

            // 目標の部屋数に達したら完了処理
            if (_currentChangeCount >= _roomChangeCount)
            {
                CompletedRunning();
                return;    
            }

            ChangeRoom();
        }

        private void CompletedRunning()
        {
            _gameEventManager.GimmickCompleted(_gimmickID);
            Returned?.Invoke(this);
            Destroyed?.Invoke();
            Destroy(gameObject);
        }

        private void ChangeRoom()
        {
            _currentChangeCount++;
            var currentRoom = _roomChecker.CheckStayRoomNum(transform.position);
            int nextRoom;
            do
            {
                nextRoom = UnityEngine.Random.Range(0, _stageGenerator.RoomCount);
            }while(currentRoom == nextRoom);

            _targetPos.x = _stageGenerator.RoomInfo[nextRoom, (int)StageGenerator.RoomStatus.CenterX];
            _targetPos.y = transform.position.y;
            _targetPos.z = _stageGenerator.RoomInfo[nextRoom, (int)StageGenerator.RoomStatus.CenterZ];
            _isMoving = true;
        }
    }
}
