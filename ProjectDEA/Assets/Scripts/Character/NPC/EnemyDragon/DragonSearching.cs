using UnityEngine;

namespace Character.NPC.EnemyDragon
{
    public class DragonSearching : MonoBehaviour
    {
        [SerializeField] private DragonController _dragonController;
        [SerializeField] private float _searchPaddingTime;
        [SerializeField] private float _searchRange;
        [SerializeField] private int _rayCount;
        [SerializeField] private float _losingDistance;
        private int _rayHalf;
        private const float RayAngleOffset = 15.0f;
        private float _currentTime;
        private GameObject _findPlayer;

        private void Start()
        {
            _rayHalf = _rayCount / 2;
        }

        private void Update()
        {
            if (_dragonController.CurrentState == AIState.Attack) return;
            _currentTime += Time.deltaTime;
            if (_currentTime < _searchPaddingTime) return;
            SearchRays();
            JudgeLoseSight();
            OnAttackState();
        }

        private void SearchRays()
        {
            var baseTransform = _dragonController.transform;
            for (var i = 0; i < _rayCount; i++)
            {
                var angle = (i - _rayHalf) * RayAngleOffset;
                var rayDirection = Quaternion.Euler(0, angle, 0) * baseTransform.forward;
                var rayOrigin = baseTransform.position;
                
                Debug.DrawRay(rayOrigin, rayDirection * _searchRange, Color.blue);

                if (!Physics.Raycast(rayOrigin, rayDirection, out var hit, _searchRange)) continue;
                if (!hit.collider.CompareTag("Player")) continue;
                _findPlayer = hit.collider.gameObject;
            }
        }

        private void JudgeLoseSight()
        {
            if (_findPlayer == null) return;
            var dis = Vector3.Distance(transform.position, _findPlayer.transform.position);
            if (_losingDistance < dis) _findPlayer = null;
        }

        private void OnAttackState()
        {
            if (_findPlayer == null) return;
            _dragonController.OnAttackState(_findPlayer.transform.position);
            _currentTime = 0;
        }
    }
}
