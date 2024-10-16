using UnityEngine;

namespace Character.NPC
{
    public class StayState : INpcAiState
    {
        private readonly Transform _npcTransform;
        private const float AngleRange = 120f;
        private const float StateTime = 5.0f;
        private const float WaitTime = 2.0f;
        private const float RotationSpeed = 1.0f;

        private float _remainTime;
        private float _waitTime;
        private Vector3 _targetDirection;
        private bool _isRotating;

        public bool IsStateFin => (_remainTime <= 0);

        public StayState(GameObject npc)
        {
            _npcTransform = npc.transform;
        }
        
        public void EnterState()
        {
            _remainTime = StateTime;
            SetDirection();
            _isRotating = true;
            _waitTime = WaitTime;
        }
        
        public void UpdateState()
        {
            _remainTime -= Time.deltaTime;

            if (_isRotating)
            {
                RotateTowardsTarget();
            }
            else
            {
                _waitTime -= Time.deltaTime;
                if (!(_waitTime <= 0)) return;
                SetDirection();
                _isRotating = true;
                _waitTime = WaitTime;
            }
        }

        public void ExitState()
        {

        }

        private void SetDirection()
        {
            var forward = _npcTransform.transform.forward;
            var randomAngle = Random.Range(-AngleRange, AngleRange);
            _targetDirection = Quaternion.Euler(0, randomAngle, 0) * forward;
        }

        private void RotateTowardsTarget()
        {
            var currentDirection = _npcTransform.forward;
            var currentRotation = _npcTransform.rotation;
            
            var targetRotation = Quaternion.LookRotation(_targetDirection);
            
            _npcTransform.rotation = Quaternion.Slerp(currentRotation, targetRotation, RotationSpeed * Time.deltaTime);
            
            if (Quaternion.Angle(currentRotation, targetRotation) < 1.0f)
            {
                _isRotating = false;
            }
        }
    }
}
