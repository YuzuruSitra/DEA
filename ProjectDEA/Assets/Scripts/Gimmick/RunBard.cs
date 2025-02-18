using System;
using Manager.Map;
using Mission;
using UnityEngine;
using UnityEngine.AI;

namespace Gimmick
{
    public class RunBard : MonoBehaviour, IInteractable, IGimmickID
    {
        [SerializeField] private int _gimmickID;
        [SerializeField] private NavMeshAgent _navMeshAgent;
        private InRoomChecker _roomChecker;
        private StageGenerator _stageGenerator;
        private GameEventManager _gameEventManager;
        public event Action<IGimmickID> Returned;
        public GimmickID GimmickIdInfo { get; set; }
        public event Action Destroyed;
        public bool IsInteractable => !_isRunning;

        [SerializeField] private int _roomChangeCount;
        private int _currentChangeCOunt;
        private bool _isRunning;
        
        
        private void Start()
        {
            _stageGenerator = GameObject.FindWithTag("StageGenerator").GetComponent<StageGenerator>();
            _gameEventManager = GameObject.FindWithTag("GameEventManager").GetComponent<GameEventManager>();
            _roomChecker = new InRoomChecker();
        }
        
        private void Update()
        {
            _isRunning = _navMeshAgent.hasPath && _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance;
        }

        public void Interact()
        {
            if (!IsInteractable) return;
            // 走破
            if (_currentChangeCOunt >= _roomChangeCount)
            {
                CompletedRuning();
                return;    
            }
            ChangeRoom();
        }

        private void CompletedRuning()
        {
            _gameEventManager.GimmickCompleted(_gimmickID);
            Returned?.Invoke(this);
            Destroyed?.Invoke();
            Destroy(gameObject);
            Debug.Log("Completed Runing");
        }

        private void ChangeRoom()
        {
            _currentChangeCOunt++;
            var currentRoom = _roomChecker.CheckStayRoomNum(gameObject.transform.position);
            var rnd = UnityEngine.Random.Range(0, 2);
            var nextRoom = currentRoom + (rnd == 0 ? -1 : 1);
            Debug.Log(nextRoom);
            nextRoom = Math.Clamp(nextRoom, 0, _stageGenerator.RoomCount - 1);
            var targetPos = Vector3.zero;
            targetPos.x = _stageGenerator.RoomInfo[nextRoom, (int)StageGenerator.RoomStatus.CenterX];
            targetPos.y = transform.position.y;
            targetPos.z = _stageGenerator.RoomInfo[nextRoom, (int)StageGenerator.RoomStatus.CenterZ];
            Debug.Log(targetPos);
            _navMeshAgent.SetDestination(targetPos);
        }   
        
    }
}
