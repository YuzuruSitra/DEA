using Character.NPC.State;
using UnityEngine;

namespace Character.NPC.Enemy.Dragon
{
	public class DragonEscape : IBattleSubState
	{
		private readonly EnemyAnimHandler _enemyAnimHandler;
		private readonly MovementControl _movementControl;
		private readonly HealthComponent _healthComponent;
		private readonly Transform _agent;
		private Transform _target;
		private readonly float _searchOffSetFactor;
		private readonly float _searchRadius;
		private readonly LayerMask _targetLayer;
		private readonly float _escapeHealth;
		private readonly float _escapeSearchRange;
		private readonly float _searchUpPadding;
		
		private readonly Collider[] _searchResults = new Collider[1];

		public DragonEscape(Transform agent, EnemyAnimHandler enemyAnimHandler, MovementControl movementControl, HealthComponent healthComponent, DragonController.ParamEscape paramEscape)
		{
			_agent = agent;
			_enemyAnimHandler  = enemyAnimHandler;
			_movementControl = movementControl;
			_healthComponent  = healthComponent;
			_searchOffSetFactor = paramEscape._searchOffSetFactor;
			_searchRadius = paramEscape._searchRadius;
			_targetLayer = paramEscape._targetLayer;
			_escapeHealth = _healthComponent.MaxHealth * paramEscape._escapeRatio;
			_searchUpPadding = paramEscape._searchUpPadding;
		}

		public float CalculateUtility()
		{
			// 仮
			var origin = _agent.position + _agent.forward * _searchOffSetFactor;
			origin.y += _searchUpPadding;
			var count = Physics.OverlapSphereNonAlloc(origin, _searchRadius, _searchResults, _targetLayer,
				QueryTriggerInteraction.Ignore);
			if (count <= 0) return 0f;
			_target = _searchResults[0].transform;
			var value = _healthComponent.CurrentHealth <= _escapeHealth ? 1.0f : 0.0f;
			return value;
		}

		public void EnterState(Transform target)
		{
			var targetPos = CalcDestination();
			_movementControl.MoveTo(targetPos);
			_movementControl.ChangeMove(true);
			_enemyAnimHandler.ChangeAnimBool(EnemyAnimHandler.AnimationBool.Moving);
		}

		public void Execute()
		{

		}

		public void ExitState()
		{

		}
		
		private Vector3 CalcDestination()
		{
			var escapeDirection = -_target.position;
			escapeDirection.y = _agent.position.y;
			var targetPos = _agent.position + escapeDirection;
            
			if (UnityEngine.AI.NavMesh.SamplePosition(targetPos, out var hit, _escapeSearchRange, UnityEngine.AI.NavMesh.AllAreas))
			{
				targetPos = hit.position;
			}
			return targetPos;
		}
		
	}
}
