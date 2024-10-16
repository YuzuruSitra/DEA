using Character.NPC.EnemyDragon;
using UnityEngine;
using UnityEngine.AI;

namespace Character.NPC
{
    public class FreeWalkState : INpcAiState
    {
        public DragonAnimCtrl.AnimState CurrentAnim { get; private set; }

        private readonly Transform _npcTransform;
        private readonly NavMeshAgent _agent;
        private readonly float _speed;
        private float _currentWait;
        private const float WaitTime = 2.0f;
        private const float Range = 5.0f;
        private const float DestinationThreshold = 1.25f;
        private readonly float _walkTimeBase;
        private readonly float _tRange;
        private float _remainTime;
        private Vector3 _rotateDirection;
        private const float WallDetectDistance = 1.0f;
        private const int RayCount = 5;
        private const float RayAngleOffset = 15.0f;
        public bool IsStateFin => (_remainTime <= 0);

        public FreeWalkState(GameObject npc, NavMeshAgent agent, float stateTimeRange, float walkTimeBase, float speed)
        {
            _npcTransform = npc.transform;
            _agent = agent;
            _tRange = stateTimeRange;
            _walkTimeBase = walkTimeBase;
            _speed = speed;
        }

        public void EnterState()
        {
            _remainTime = Random.Range(_walkTimeBase - _tRange, _walkTimeBase + _tRange);
            _agent.isStopped = false;
            SetRandomDestination();
            _agent.speed = _speed;
            CurrentAnim = DragonAnimCtrl.AnimState.IsWalk;
            _currentWait = WaitTime;
        }

        public void UpdateState()
        {
            _remainTime -= Time.deltaTime;

            // 壁に近いかを検知
            if (IsNearWall())
            {
                // 壁に当たった場合、1/3の確率でステートを終了
                if (Random.value <= 1.0f / 3.0f)
                {
                    //_remainTime = 0; // ステートを強制終了
                    return;
                }

                _currentWait = 0;
                SetRandomDestination(); // 回避を開始
            }

            if (!_agent.pathPending && _agent.remainingDistance <= DestinationThreshold)
                SetRandomDestination();
        }

        public void ExitState()
        {
            _agent.isStopped = true;
            _agent.speed = 0;
            CurrentAnim = DragonAnimCtrl.AnimState.Idole;
        }

        // ランダムな通れる目的地を設定
        private void SetRandomDestination()
        {
            _currentWait -= Time.deltaTime;
            if (_currentWait >= 0) return;
            
            var randomDirection = Random.insideUnitSphere * Range; // ランダムな方向を生成
            randomDirection += _npcTransform.position; // NPCの位置を基準にする

            // NavMesh上の通れる位置を取得
            if (NavMesh.SamplePosition(randomDirection, out var hit, Range, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position); // 通れる位置を目的地に設定
            }

            _currentWait = WaitTime;
        }

        // 複数方向への Raycast で壁に近いかを検知
        private bool IsNearWall()
        {
            var nearWall = false;
            for (var i = 0; i < RayCount; i++)
            {
                var angle = (i - 2) * RayAngleOffset;
                var rayDirection = Quaternion.Euler(0, angle, 0) * _npcTransform.forward;
                var rayOrigin = _npcTransform.position;

                // Rayを描画する
                Debug.DrawRay(rayOrigin, rayDirection * WallDetectDistance, Color.red);

                if (_agent.Raycast(rayOrigin + rayDirection * WallDetectDistance, out var hit) && hit.distance < WallDetectDistance)
                {
                    nearWall = true;
                }
            }
            return nearWall;
        }
    }
}
