using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SmartGuardWithModel : MonoBehaviour
{
    public Transform player;
    public float checkInterval = 0.5f;
    public float pointReachedThreshold = 0.2f;

    public GameObject frontModel;
    public GameObject backModel;
    public GameObject sideModel;

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

    void Update()
    {
        Vector3 velocity = agent.velocity;
        if (velocity.sqrMagnitude > 0.01f)
        {
            UpdateModelDirection(velocity);
        }
    }

    void UpdateModelDirection(Vector3 velocity)
    {
        if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
        {
            ShowOnlyModel(sideModel);

            if (velocity.x > 0)
                sideModel.transform.localScale = new Vector3(-1, 1, 1); // שמאלה
            else
                sideModel.transform.localScale = new Vector3(1, 1, 1); // ימינה

        }
        else
        {
            if (velocity.y > 0)
                ShowOnlyModel(backModel);
            else
                ShowOnlyModel(frontModel);
        }
    }

    void ShowOnlyModel(GameObject modelToShow)
    {
        frontModel.SetActive(modelToShow == frontModel);
        backModel.SetActive(modelToShow == backModel);
        sideModel.SetActive(modelToShow == sideModel);
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
