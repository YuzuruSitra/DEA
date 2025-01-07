using System;
using System.Collections.Generic;
using UnityEngine;

namespace Test.NPC.Dragon
{
	public class DragonController : NpcController
	{
		[Serializable]
		public struct AttackParameters
		{
			public Vector3 _searchOffSet;
			public float _searchRadius;
			public LayerMask _targetLayer;
			public float _attackRadius;
			public float _attackDelay;
			public float _attackDamage;
			public Vector3 _attackOffSet;
		}
		[SerializeField] private AttackParameters _attackParameters;
		
		// Roaming logic
		[Serializable]
		public struct RoamingParameters
		{
			public float _intervalTimeMax;
			public float _intervalTimeMin;
			public float _roamingSearchRange;
		}
		[SerializeField] private RoamingParameters _roamingParameters;
		
		protected override void Start()
		{
			base.Start();
			// ActionSelector にドラゴン固有のアクションを追加
			ActionSelector = new ActionSelector(new List<IUtilityAction>
			{
				new AttackAction(transform, AnimatorControl, MovementControl, _attackParameters),
				new RestAction(transform, AnimatorControl, MovementControl),
				new RoamingAction(transform, AnimatorControl, MovementControl, _roamingParameters)
			});
		}
	}
}