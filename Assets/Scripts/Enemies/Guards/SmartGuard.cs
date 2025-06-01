using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SmartGuard : MonoBehaviour
{
    public Transform player;
    public float checkInterval = 0.5f;
    public float pointReachedThreshold = 0.2f;

    private NavMeshAgent agent;
    private RoomTracker guardTracker;
    private Room currentTargetRoom;
    private int patrolIndex = 0;
    private enum State { Idle, Traveling, Patrolling }
    private State state = State.Idle;

    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        guardTracker = GetComponent<RoomTracker>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        StartCoroutine(StateLoop());
    }

    IEnumerator StateLoop()
    {
        while (true)
        {
            switch (state)
            {
                case State.Idle:
                    yield return new WaitForSeconds(checkInterval);
                    ChooseTargetRoom();
                    break;

                case State.Traveling:
                    if (ReachedDestination())
                    {
                        state = State.Patrolling;
                        patrolIndex = 0;
                        SetNextPatrolPoint();
                    }
                    break;

                case State.Patrolling:
                    if (ReachedDestination())
                    {
                        patrolIndex++;
                        Room room = guardTracker.currentRoom;

                        if (room != null && room.patrolPoints != null && patrolIndex < room.patrolPoints.Length)
                        {
                            SetNextPatrolPoint();
                        }
                        else
                        {
                            state = State.Idle;
                        }
                    }
                    break;
            }

            yield return null;
        }
    }

    void ChooseTargetRoom()
    {
        RoomTracker playerTracker = player.GetComponent<RoomTracker>();
        if (playerTracker != null && playerTracker.currentRoom != null)
        {
            currentTargetRoom = playerTracker.currentRoom;
            agent.SetDestination(currentTargetRoom.transform.position);
            state = State.Traveling;
        }
    }

    void SetNextPatrolPoint()
    {
        Room room = guardTracker.currentRoom;
        if (room != null && room.patrolPoints != null && patrolIndex < room.patrolPoints.Length)
        {
            agent.SetDestination(room.patrolPoints[patrolIndex].position);
        }
    }

    bool ReachedDestination()
    {
        return !agent.pathPending && agent.remainingDistance <= pointReachedThreshold;
    }
}
