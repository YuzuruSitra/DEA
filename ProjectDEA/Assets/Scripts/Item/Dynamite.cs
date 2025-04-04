using System.Collections;
using Character.NPC;
using Character.Player;
using Gimmick.BreakRock;
using Manager.Audio;
using Manager.MetaAI;
using UnityEngine;

namespace Item
{
    public class Dynamite : MonoBehaviour
    {
        [SerializeField] private float _detonationTime;
        [SerializeField] private float _rayLength;
        [SerializeField] private float _animPaddingTime;
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationClip _attackClip;
        private float _animWaitTime;
        private static readonly int Bomb = Animator.StringToHash("Bomb");
        [SerializeField] private LayerMask _layerToIgnore;
        [SerializeField] private int _discoveryRate;

        [SerializeField] private int _playerGiveDamage;
        private bool _isPlayerGive;
        [SerializeField] private int _giveDamage;
        private bool _isDragonGive;
        private bool _isObjGive;
        private bool _isPutMonument;
        private SoundHandler _soundHandler;
        [SerializeField] private AudioClip _bombAudio;
        private MetaAIHandler _metaAIHandler;
        [SerializeField] private MetaAIHandler.AddScores[] _bombScores;
        
        private void Start()
        {
            _soundHandler = GameObject.FindWithTag("SoundHandler").GetComponent<SoundHandler>();
            _metaAIHandler = GameObject.FindWithTag("MetaAI").GetComponent<MetaAIHandler>();
            UseEffect();
        }

        private void UseEffect()
        {
            _animWaitTime = _attackClip.length - _animPaddingTime;
            _detonationTime -= _animWaitTime;
            StartCoroutine(BombEffect());
            _metaAIHandler.SendLogsForMetaAI(_bombScores);
        }

        private IEnumerator BombEffect()
        {
            yield return new WaitForSeconds(_detonationTime);
            _animator.SetTrigger(Bomb);
            yield return new WaitForSeconds(_animWaitTime - 0.2f);
            _soundHandler.PlaySe(_bombAudio);
            yield return new WaitForSeconds(0.2f);
            var directions = SetDirections();
            var adPos = AdjustedPosition();   
            
            PerformRaycastInDirections(directions, adPos);
            PerformRaycastInDirections(directions, adPos + Vector3.up);
            yield return new WaitForSeconds(_animPaddingTime);
            Destroy(gameObject);
        }

        private void PerformRaycastInDirections(Vector3[] directions, Vector3 origin)
        {
            foreach (var direction in directions)
            {
                var ray = new Ray(origin, direction);
                var hits = Physics.RaycastAll(ray, _rayLength, ~_layerToIgnore);
                
                foreach (var hit in hits)
                {
                    var obj = hit.collider.gameObject;
                    
                    if (obj.CompareTag("Player"))
                    {
                        if (_isPlayerGive) continue;
                        _isPlayerGive = true;
                        var playerHub = obj.GetComponent<PlayerClasHub>();
                        if (playerHub == null) continue;
                        if (!playerHub.PlayerHpHandler.IsAddDamage) return;
                        playerHub.PlayerHpHandler.ReceiveDamage(_playerGiveDamage);
                        continue;
                    }
                    
                    if (obj.CompareTag("Enemy"))
                    {
                        if (_isDragonGive) continue;
                        _isDragonGive = true;
                        var enemyController = obj.GetComponent<NpcController>();
                        if (enemyController == null) continue;
                        enemyController.OnGetDamage(_giveDamage);
                        continue;
                    }

                    if (obj.CompareTag("BreakRock"))
                    {
                        if (_isObjGive) continue;
                        var breakRock = obj.GetComponent<BreakRock>();
                        if (breakRock == null) continue;
                        breakRock.OnGetDamage(_giveDamage);
                        _isObjGive = true;
                    }
                }
            }
        }
        
        private static Vector3[] SetDirections()
        {
            var directions = new Vector3[9];
            directions[0] = Vector3.forward;     // +Z 方向
            directions[1] = Vector3.back;        // -Z 方向
            directions[2] = Vector3.right;       // +X 方向
            directions[3] = Vector3.left;        // -X 方向
            directions[4] = Vector3.up;
            directions[5] = (Vector3.forward + Vector3.right).normalized;  // +Z +X 方向
            directions[6] = (Vector3.forward + Vector3.left).normalized;   // +Z -X 方向
            directions[7] = (Vector3.back + Vector3.right).normalized;     // -Z +X 方向
            directions[8] = (Vector3.back + Vector3.left).normalized;      // -Z -X 方向
            return directions;
        }

        private Vector3 AdjustedPosition()
        {
            var adjustedPosition = Vector3.zero;
            adjustedPosition.x = transform.position.x;
            adjustedPosition.y = Mathf.Round(transform.position.y * 2) / 2;
            adjustedPosition.z = transform.position.z;
            return adjustedPosition;
        }
    }
}
