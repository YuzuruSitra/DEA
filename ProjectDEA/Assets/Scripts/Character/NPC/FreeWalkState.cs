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
        private bool _isAvoiding;
        private bool _isRotating;
        private Quaternion _targetRotation;
        private const float RotationSpeed = 0.75f;
        private float _stopTime; // Timer for stopping after hitting a wall

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
            _agent.isStopped = false; // Initially stop the agent until rotation is complete
            _isAvoiding = false;
            _isRotating = false;
            _currentWait = 0;
            _stopTime = 0; // Reset stop timer
            SetRandomDestination();
            _agent.speed = _speed;
            CurrentAnim = DragonAnimCtrl.AnimState.IsWalk;
            _currentWait = WaitTime;
        }

        public void UpdateState()
        {
            _remainTime -= Time.deltaTime;

            // If stopped after hitting a wall, count down the stop time
            if (_stopTime > 0)
            {
                _stopTime -= Time.deltaTime;
                if (_stopTime <= 0)
                {
                    _agent.isStopped = false; // Resume movement after 1 second stop
                }
                return;
            }

            // Rotate towards target before moving
            if (_isRotating)
            {
                RotateTowardsTarget();
                return;
            }

            // Detect if near a wall
            if (IsNearWall())
            {
                if (_isAvoiding) return;
                _isAvoiding = true;

                _stopTime = 1.0f; // Set stop timer for 1 second
                _agent.isStopped = true; // Stop the agent temporarily

                SetAvoidDestination(); // Start avoiding after pause
            }

            if (_isAvoiding) return;

            if (!_agent.pathPending && _agent.remainingDistance <= DestinationThreshold)
            {
                SetRandomDestination(); // Set new destination once current is reached
            }
        }

        public void ExitState()
        {
            _agent.isStopped = true;
            _agent.speed = 0;
            CurrentAnim = DragonAnimCtrl.AnimState.Idole;
        }

        private void SetRandomDestination()
        {
            _currentWait -= Time.deltaTime;
            if (_currentWait > 0) return;
            
            var randomDirection = Random.insideUnitSphere * Range; 
            randomDirection += _npcTransform.position; 

            if (NavMesh.SamplePosition(randomDirection, out var hit, Range, NavMesh.AllAreas))
            {
                _targetRotation = Quaternion.LookRotation(hit.position - _npcTransform.position);
                _isRotating = true; // Start rotating
                _agent.SetDestination(hit.position);
            }

            _currentWait = WaitTime;
        }

        private void SetAvoidDestination()
        {
            var randomDirection = Random.insideUnitSphere;
            randomDirection.y = 0;
    
            var npcBackward = -_npcTransform.forward;
            if (Vector3.Dot(randomDirection, npcBackward) < 0)
            {
                randomDirection = -randomDirection;
            }

            randomDirection = randomDirection.normalized * Range;
            randomDirection += _npcTransform.position;

            if (NavMesh.SamplePosition(randomDirection, out var hit, Range, NavMesh.AllAreas))
            {
                _targetRotation = Quaternion.LookRotation(hit.position - _npcTransform.position);
                _isRotating = true; // Start rotating before moving to avoid destination
                _agent.SetDestination(hit.position);
            }

            _currentWait = WaitTime;
        }

        private bool IsNearWall()
        {
            for (var i = 0; i < RayCount; i++)
            {
                var angle = (i - 2) * RayAngleOffset;
                var rayDirection = Quaternion.Euler(0, angle, 0) * _npcTransform.forward * 1.5f;
                var rayOrigin = _npcTransform.position;

                Debug.DrawRay(rayOrigin, rayDirection * WallDetectDistance, Color.red);

                if (_agent.Raycast(rayOrigin + rayDirection * WallDetectDistance, out var hit) && hit.distance < WallDetectDistance)
                {
                    return true;
                }
            }

            _isAvoiding = false;
            return false;
        }

        private void RotateTowardsTarget()
        {
            _npcTransform.rotation = Quaternion.Slerp(_npcTransform.rotation, _targetRotation, Time.deltaTime * RotationSpeed);
            _agent.isStopped = true;
            if (!(Quaternion.Angle(_npcTransform.rotation, _targetRotation) < 35f)) return; // Close enough to target direction
            _isRotating = false;
            _agent.isStopped = false; // Start moving after rotation is done
        }
    }
}
