using UnityEngine;
using UnityEngine.AI;

public class SmartGuard : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 5f;
    public float minDistance = 2f;
    public float updateInterval = 0.5f;
    private float timer = 0f;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }


    void Update()
    {
        timer += Time.deltaTime;
        if (timer > updateInterval)
        {
            timer = 0f;
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance < detectionRadius && distance > minDistance)
            {
                agent.SetDestination(player.position);
                agent.speed = 1.0f;
            }
            else
            {
                agent.SetDestination(transform.position);
            }
        }
    }
}
