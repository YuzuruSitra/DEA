using Character.NPC.EnemyDragon;
using Gimmick;
using Manager;
using Manager.Audio;
using UnityEngine;

namespace Character.Player
{
    public class PlayerSwards : MonoBehaviour
    {
        [SerializeField] private PlayerAttackHandler _playerAttackHandler;
        private Transform _playerTrn;
        private bool _oneHit;
        
        private GameObject _currentDragon;
        private DragonController _currentDragonController;
        
        private GameObject _currentBorn;
        private BornOut _currentBornOut;
        
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _attackHitSeClip;
        private bool _oneChange;
        private PlayerStatusHandler _playerStatusHandler;
        
        private void Start()
        {
            _playerTrn = _playerAttackHandler.gameObject.transform;
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
            _playerStatusHandler = GameObject.FindWithTag("PlayerStatusHandler").GetComponent<PlayerStatusHandler>();
        }

        private void Update()
        {
            // 攻撃を終了したら当たり判定をリセット
            if (!_oneHit) return;
            _oneChange = _playerAttackHandler.IsAttacking;
            if (_oneChange) return;
            _oneHit = false;
            _oneChange = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_playerAttackHandler.IsAttacking)
            {
                _oneHit = false;
                return;
            }
            if (_oneHit) return;
            
            if (other.CompareTag("EnemyDragon"))
            {
                if (_currentDragon != other.gameObject)
                {
                    _currentDragon = other.gameObject;
                    _currentDragonController = _currentDragon.GetComponent<DragonController>();
                }

                _currentDragonController.OnGetDamage(_playerStatusHandler.PlayerAttackDamage, _playerTrn.position);
                _soundHandler.PlaySe(_attackHitSeClip);
                _oneHit = true;
            }

            if (other.CompareTag("BornOut"))
            {
                if (_currentBorn != other.gameObject)
                {
                    _currentBorn = other.gameObject;
                    _currentBornOut = _currentBorn.GetComponent<BornOut>();
                }
                _currentBornOut.FlyAwayBorn(_playerTrn.position);
                _soundHandler.PlaySe(_attackHitSeClip);
                _oneHit = true;
            }
            
        }
    }
}
