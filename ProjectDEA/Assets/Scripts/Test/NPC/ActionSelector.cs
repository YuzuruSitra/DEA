using System.Collections.Generic;
using UnityEngine;

namespace Test.NPC
{
	public class ActionSelector
	{
		private readonly List<IUtilityAction> _actions;

		public ActionSelector(List<IUtilityAction> actions)
		{
			_actions = actions;
		}

		public IUtilityAction SelectBestAction()
		{
			IUtilityAction bestAction = null;
			float highestUtility = 0;

			foreach (var action in _actions)
			{
				var utility = action.CalculateUtility();
				if (!(utility > highestUtility)) continue;
				highestUtility = utility;
				bestAction = action;
			}

			return bestAction;
		}
	}
}