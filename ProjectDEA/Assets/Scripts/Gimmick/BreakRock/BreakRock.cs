using System;
using Character.NPC;
using Manager.MetaAI;
using UnityEngine;

namespace Gimmick.BreakRock
{
    public class BreakRock : MonoBehaviour, IGimmickID
    {
        public GimmickID GimmickIdInfo { get; set; }
        public event Action<IGimmickID> Returned;
        [SerializeField] private float _maxHealth;
        private HealthComponent _healthComponent;
        [SerializeField] private float _playerReductionRatio;
        public float PlayerReductionRatio => _playerReductionRatio;
        private MetaAIHandler _metaAIHandler;
        [SerializeField] private MetaAIHandler.AddScores[] _metaAiScores;
        [SerializeField] private RockHpGaugeHandler _rockHpGaugeHandler;
        [Serializable]
        public struct InsInfo
        {
            [Header("生成リストから除去するか否か")]
            public bool _isRetuned;
            public GameObject _prefab;
        }
        [SerializeField] private InsInfo[] _insGameObject;
        
        private void Start()
        {
            _healthComponent = new HealthComponent(_maxHealth);
            _metaAIHandler = GameObject.FindWithTag("MetaAI").GetComponent<MetaAIHandler>();
            _rockHpGaugeHandler.InitialSet(_healthComponent.MaxHealth, _healthComponent.CurrentHealth);
            _healthComponent.OnDeath += OnBreak;
            _healthComponent.OnHealthChanged += _rockHpGaugeHandler.ChangeGauge;
        }
        
        private void OnDestroy()
        {
            _healthComponent.OnDeath -= OnBreak;
            _healthComponent.OnHealthChanged -= _rockHpGaugeHandler.ChangeGauge;
        }

        public void OnGetDamage(int damage)
        {
            if (_healthComponent.CurrentHealth <= 0) return;
            _metaAIHandler.SendLogsForMetaAI(_metaAiScores);
            _healthComponent.TakeDamage(damage);
        }

        private void OnBreak()
        {
            Destroy(gameObject);
            if (_insGameObject.Length == 0)
            {
                Returned?.Invoke(this);
                return;
            }
            var target = UnityEngine.Random.Range(0, _insGameObject.Length);
            if (_insGameObject[target]._isRetuned)
            {
                Returned?.Invoke(this);
            }
            Instantiate(_insGameObject[target]._prefab, transform.position, Quaternion.identity);
        }
    }
}
