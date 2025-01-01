using UnityEngine;
using UnityEngine.AI;

namespace Test.NPC
{
	public class MovementControl : MonoBehaviour
	{
		private NavMeshAgent _agent;

		private void Awake()
		{
			_agent = GetComponent<NavMeshAgent>();
		}

		public void MoveTo(Vector3 destination)
		{
			if (_agent != null)
			{
				_agent.SetDestination(destination);
			}
		}

		public void Stop()
		{
			if (_agent != null)
			{
				_agent.isStopped = true;
			}
		}

		public bool HasReachedDestination()
		{
			if (_agent == null) return false;
			return !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance;
		}
	}
}