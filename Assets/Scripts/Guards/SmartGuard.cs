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
    private float originalSpeed;

    private NavMeshAgent agent;
    private RoomTracker guardTracker;
    private Room currentTargetRoom;
    private int patrolIndex = 0;
    private enum State { Idle, Traveling, Patrolling }
    private State state = State.Idle;
    private bool isInAlert = false;

    private enum Direction { Front, Back, Side }
    private Direction lastDirection = Direction.Side;

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

        originalSpeed = agent.speed;

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
            lastDirection = Direction.Side;
        }
        else if (velocity.z > 0)
        {
            ShowModel(backModel);
            SetAnimation(backAnim, isMoving ? "walking back" : "idle back");
            lastDirection = Direction.Back;
        }
        else
        {
            ShowModel(frontModel);
            SetAnimation(frontAnim, isMoving ? "walking front" : "idle front");
            lastDirection = Direction.Front;
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
        if (anim != null && anim.AnimationName != animationName)
        {
            anim.AnimationName = animationName;
        }
    }

    public void ReactToStolenItem()
    {
        if (!isInAlert)
        {
            StartCoroutine(AlarmThenRunRoutine());
        }
    }

    private IEnumerator AlarmThenRunRoutine()
    {
        isInAlert = true;

        agent.isStopped = true;
        PlayAlarmAnimation();
        yield return new WaitForSeconds(2f);

        agent.isStopped = false;
        agent.speed = originalSpeed * 1.5f;
        PlayRunAnimation();
        yield return new WaitForSeconds(5f);

        agent.speed = originalSpeed;
        isInAlert = false;
    }

    void PlayAlarmAnimation()
    {
        switch (lastDirection)
        {
            case Direction.Side:
                ShowModel(sideModel);
                SetAnimation(sideAnim, "alarm");
                break;
            case Direction.Back:
                ShowModel(backModel);
                SetAnimation(backAnim, "alarm");
                break;
            case Direction.Front:
                ShowModel(frontModel);
                SetAnimation(frontAnim, "alarm");
                break;
        }
    }

    void PlayRunAnimation()
    {
        switch (lastDirection)
        {
            case Direction.Side:
                ShowModel(sideModel);
                SetAnimation(sideAnim, "run");
                break;
            case Direction.Back:
                ShowModel(backModel);
                SetAnimation(backAnim, "run");
                break;
            case Direction.Front:
                ShowModel(frontModel);
                SetAnimation(frontAnim, "run");
                break;
        }
    }
}
