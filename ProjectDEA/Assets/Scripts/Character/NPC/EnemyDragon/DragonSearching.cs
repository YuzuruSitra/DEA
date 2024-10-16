using System;
using UnityEngine;

namespace Character.NPC.EnemyDragon
{
    public class DragonSearching : MonoBehaviour
    {
        [SerializeField] private DragonController _dragonController;
        [SerializeField] private float _searchPaddingTime;
        private float _currentTime;

        private void Update()
        {
            if (_dragonController.CurrentState == AIState.Attack) return;
            _currentTime += Time.deltaTime;
        }

        private void OnTriggerStay(Collider other)
        {
            if (_currentTime < _searchPaddingTime) return;
            if (!other.CompareTag("Player")) return;
            _dragonController.OnAttackState(other.transform.position);
            _currentTime = 0;
        }
    }
}
