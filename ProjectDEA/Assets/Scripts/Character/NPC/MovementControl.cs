using UnityEngine;
using UnityEngine.AI;

namespace Character.NPC
{
	public class MovementControl
	{
		private readonly NavMeshAgent _agent;

		public MovementControl(NavMeshAgent agent)
		{
			_agent = agent;
		}

		public void MoveTo(Vector3 destination)
		{
			if (_agent != null)
			{
				_agent.SetDestination(destination);
			}
		}

		public void ChangeMove(bool isMove)
		{
			if (!isMove)
			{
				_agent.velocity = Vector3.zero;
			}
			_agent.isStopped = !isMove;
		}

		public bool HasReachedDestination()
		{
			if (_agent == null) return false;
			return !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance;
		}
	}
}