using System.Collections;
using Test.NPC.Dragon;
using UnityEngine;

namespace Test.NPC
{
    public class AttackAction : IUtilityAction
    {
        public UtilityActionType ActionType => UtilityActionType.Attack;
        private readonly AnimatorControl _animatorControl;
        private readonly MovementControl _movementControl;
        private readonly Vector3 _searchOffSet;
        private readonly float _searchRadius;
        private readonly LayerMask _searchLayer;
        private readonly float _attackRadius;
        private readonly float _attackDelay;
        private readonly float _damage;
        private readonly Vector3 _attackOrigin;
        
        private readonly Transform _agent;
        private Transform _target;
        private DebugDrawCd _debugDrawCd;
        
        public AttackAction(Transform agent, AnimatorControl animatorControl, MovementControl movementControl, DragonController.AttackParameters attackParameters)
        {
            _agent = agent;
            _animatorControl = animatorControl;
            _movementControl = movementControl;
            _searchOffSet = attackParameters._searchOffSet;
            _searchRadius = attackParameters._searchRadius;
            _searchLayer = attackParameters._searchLayer;
            _attackRadius = attackParameters._attackRadius;
            _attackDelay = attackParameters._attackDelay;
            _damage = attackParameters._attackDamage;
            _attackOrigin = attackParameters._attackOffSet;
            _debugDrawCd = new DebugDrawCd();
        }

        public float CalculateUtility()
        {
            var center = _agent.position + _searchOffSet;
            var results = new Collider[1];
            var count = Physics.OverlapSphereNonAlloc(center, _searchRadius, results, _searchLayer, QueryTriggerInteraction.Ignore);
            if (count <= 0) return 0f;
            Debug.Log(results[0].gameObject.name);
            _target = results[0].transform;
            return 1f;
        }

        public void Execute(GameObject agent)
        {
            _debugDrawCd.DebugDrawSphere(_agent.position + _searchOffSet, _searchRadius, Color.blue);
            _debugDrawCd.DebugDrawSphere(agent.transform.position + _attackOrigin, _attackRadius, Color.red);
            if (_target == null) return;
            // 攻撃のCT待機
            
            // 対象の元へ移動
            
            // 対象を攻撃
            
            // 逃避
            
            
            // アニメーションを再生しつつ、攻撃判定を実行
            // _animatorControl.SetTrigger("Attack");
            // agent.StartCoroutine(PerformAttack(agent));
        }

        private IEnumerator PerformAttack(GameObject agent)
        {
            yield return new WaitForSeconds(_attackDelay); // 攻撃が発生するタイミングまで待機
                
            // 攻撃範囲内のオブジェクトを取得
            var hitColliders = Physics.OverlapSphere(agent.transform.position + _attackOrigin, _attackRadius);

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject == agent) continue; // 自身を除外

                var targetHealth = hitCollider.GetComponent<HealthComponent>();
                if (targetHealth == null) continue;
                targetHealth.TakeDamage(_damage);
                Debug.Log($"Hit {hitCollider.name} for {_damage} damage.");
            }
        }

    }
}
