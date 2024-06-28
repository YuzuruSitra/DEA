using UnityEngine;
using UnityEngine.AI;

public class NPCMover : MonoBehaviour
{
    public Transform goal;
    NavMeshAgent _agent;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void Update() 
    {
        _agent.destination = goal.position;
    }
}
