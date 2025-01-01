using System;
using Character.NPC.EnemyDragon;
using UnityEngine;

namespace Gimmick
{
    public class EnemySpawnArea : MonoBehaviour, IGimmickID
    {
        [SerializeField] private GameObject _targetDragon;
        private Character.NPC.EnemyDragon.DragonController _dragonController;
        public GimmickID GimmickIdInfo { get; set; }
        public event Action<IGimmickID> Returned;
        private bool _oneTime;

        private void Start()
        {
            _dragonController = Instantiate(_targetDragon, transform.position, Quaternion.identity).GetComponent<Character.NPC.EnemyDragon.DragonController>();
        }

        private void Update()
        {
            if(!_dragonController.IsDeath) return;
            if(_oneTime) return;
            _oneTime = true;
            Returned?.Invoke(this);
            Destroy(gameObject);
        }
        
    }
}
