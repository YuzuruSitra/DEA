using System.Collections.Generic;

namespace Test.NPC.State
{
    public class BattleStateSelector
    {
        private readonly List<IBattleSubState> _actions;

        public BattleStateSelector(List<IBattleSubState> actions)
        {
            _actions = actions;
        }

        public IBattleSubState SelectBestAction()
        {
            IBattleSubState bestAction = null;
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
