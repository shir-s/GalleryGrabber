using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Spine.Unity;

public class SmartGuard : MonoBehaviour
{
    public Transform player;
    public float checkInterval = 0.5f;
    public float pointReachedThreshold = 0.2f;

    [Header("Spine Animation Models")]
    public GameObject frontModel;
    public GameObject backModel;
    public GameObject sideModel;

    private SkeletonAnimation frontAnim;
    private SkeletonAnimation backAnim;
    private SkeletonAnimation sideAnim;

    private float baseScaleX;

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

        frontAnim = frontModel.GetComponent<SkeletonAnimation>();
        backAnim = backModel.GetComponent<SkeletonAnimation>();
        sideAnim = sideModel.GetComponent<SkeletonAnimation>();
        baseScaleX = sideAnim.Skeleton.ScaleX;

        StartCoroutine(StateLoop());
        
        Debug.Log("Initial ScaleX: " + sideAnim.Skeleton.ScaleX);

        
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

    void Update()
    {
        UpdateAnimation();
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

    void UpdateAnimation()
    {
        Vector3 velocity = agent.velocity;
        bool isMoving = velocity.magnitude > 0.1f;

        if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.z))
        {
            ShowModel(sideModel);
            SetAnimation(sideAnim, isMoving ? "walking side" : "idle side");

            sideAnim.Skeleton.ScaleX = velocity.x > 0 ? -baseScaleX : baseScaleX;
        }
        else if (velocity.z > 0)
        {
            ShowModel(backModel);
            SetAnimation(backAnim, isMoving ? "walking back" : "idle back");
        }
        else
        {
            ShowModel(frontModel);
            SetAnimation(frontAnim, isMoving ? "walking front" : "idle front");
        }
    }


    void ShowModel(GameObject activeModel)
    {
        frontModel.SetActive(activeModel == frontModel);
        backModel.SetActive(activeModel == backModel);
        sideModel.SetActive(activeModel == sideModel);
    }

    void SetAnimation(SkeletonAnimation anim, string animationName)
    {
        if (anim.AnimationName != animationName)
        {
            anim.AnimationName = animationName;
        }
    }
}
