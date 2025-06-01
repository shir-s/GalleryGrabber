using UnityEngine;
using UnityEngine.AI;

public class PatrolGuard : MonoBehaviour
{
    public Transform[] waypoints;
    public float reachDistance = 0.2f;

    NavMeshAgent agent;
    int index = 0;

    
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis  = false;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (waypoints.Length > 0)
            agent.SetDestination(waypoints[0].position);
    }

    void Update()
    {
        if (waypoints.Length == 0) return;
        if (agent.pathPending)       return;

        if (agent.remainingDistance <= reachDistance)
        {
            index = (index + 1) % waypoints.Length;
            agent.SetDestination(waypoints[index].position);
        }
    }
}

