using Character.NPC.EnemyDragon;
using UnityEngine;

namespace Gimmick
{
    public class EnemySpawnArea : MonoBehaviour, IGimmickID
    {
        [SerializeField] private DragonController _targetDragon;
        public int InRoomID { get; set; }

        private void Start()
        {
            Instantiate(_targetDragon.gameObject, transform.position, Quaternion.identity);
        }
        
    }
}
