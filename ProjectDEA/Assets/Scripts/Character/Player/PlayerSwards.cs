using Character.NPC;
using Gimmick;
using Gimmick.BreakRock;
using Item;
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
        
        private GameObject _currentEnemy;
        private NpcController _currentNpcController;
        
        private GameObject _currentbreakRock;
        private BreakRock _currentBreakRock;
        
        private GameObject _currentBorn;
        private BornOut _currentBornOut;

        private GameObject _currentRock;
        private HardRock _currentHardRock;
        
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
            
            if (other.CompareTag("Enemy"))
            {
                if (_currentEnemy != other.gameObject)
                {
                    _currentEnemy = other.gameObject;
                    _currentNpcController = _currentEnemy.GetComponent<NpcController>();
                }
                if (_currentNpcController == null) return;
                _currentNpcController.OnGetDamage(_playerStatusHandler.PlayerAttackDamage);
                _soundHandler.PlaySe(_attackHitSeClip);
                _oneHit = true;
            }
                
            if (other.CompareTag("BreakRock"))
            {
                if (_currentbreakRock != other.gameObject)
                {
                    _currentbreakRock = other.gameObject;
                    _currentBreakRock = other.gameObject.GetComponent<BreakRock>();
                }
                var damage = _playerStatusHandler.PlayerAttackDamage * _currentBreakRock.PlayerReductionRatio;
                _currentBreakRock.OnGetDamage((int)damage);
                _soundHandler.PlaySe(_attackHitSeClip);
                _oneHit = true;
            }

            if (other.CompareTag("VenomMush"))
            {
                Destroy(other.gameObject);
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
            
            if (other.CompareTag("HardRock"))
            {
                if (_currentRock != other.gameObject)
                {
                    _currentRock = other.gameObject;
                    _currentHardRock = _currentRock.GetComponent<HardRock>();
                }
                _currentHardRock.FlyAwayStone(_playerTrn.position);
                _soundHandler.PlaySe(_attackHitSeClip);
                _oneHit = true;
            }
            
        }
    }
}
