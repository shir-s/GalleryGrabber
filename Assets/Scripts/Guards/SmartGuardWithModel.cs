using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Spine.Unity;

public class SmartGuardWithModel : MonoBehaviour
{
    public Transform player;
    public float checkInterval = 0.5f;
    public float pointReachedThreshold = 0.2f;

    public GameObject frontModel;
    public GameObject backModel;
    public GameObject sideModel;

    [SerializeField] private GameObject alarmDialog;
    private SkeletonAnimation frontAnim;
    private SkeletonAnimation backAnim;
    private SkeletonAnimation sideAnim;

    private NavMeshAgent agent;
    private RoomTracker guardTracker;
    private Room currentTargetRoom;
    private int patrolIndex = 0;

    private enum State { Idle, Traveling, Patrolling }
    private State state = State.Idle;

    private bool isPaused = false;
    private float pauseTimer = 0f;
    public float pauseDuration = 2f;

    private bool isInAlert = false;
    private Coroutine alertRoutine = null;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        guardTracker = GetComponent<RoomTracker>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        frontAnim = frontModel.GetComponent<SkeletonAnimation>();
        backAnim = backModel.GetComponent<SkeletonAnimation>();
        sideAnim = sideModel.GetComponent<SkeletonAnimation>();

        StartCoroutine(StateLoop());
    }

    void Update()
    {
        if (isPaused || isInAlert)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f && isPaused)
            {
                isPaused = false;
                agent.isStopped = false;

                if (state == State.Patrolling)
                    SetNextPatrolPoint();
                else if (state == State.Traveling)
                    agent.SetDestination(currentTargetRoom.transform.position);
            }
            return;
        }

        Vector3 velocity = agent.velocity;
        bool isMoving = velocity.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            UpdateModelDirection(velocity);
        }

        if (sideModel.activeSelf)
            SetAnimation(sideAnim, isMoving ? "walking side" : "idle side");
        else if (backModel.activeSelf)
            SetAnimation(backAnim, isMoving ? "walking back" : "idle back");
        else if (frontModel.activeSelf)
            SetAnimation(frontAnim, isMoving ? "walking front" : "idle front");
    }

    void UpdateModelDirection(Vector3 velocity)
    {
        if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
        {
            ShowOnlyModel(sideModel);
            sideModel.transform.localScale = velocity.x > 0 ? Vector3.one : new Vector3(-1, 1, 1);
        }
        else
        {
            ShowOnlyModel(velocity.y > 0 ? backModel : frontModel);
        }
    }

    void ShowOnlyModel(GameObject modelToShow)
    {
        frontModel.SetActive(modelToShow == frontModel);
        backModel.SetActive(modelToShow == backModel);
        sideModel.SetActive(modelToShow == sideModel);
    }

    void SetAnimation(SkeletonAnimation anim, string animationName)
    {
        if (anim != null && anim.AnimationName != animationName)
            anim.AnimationName = animationName;
    }

    IEnumerator StateLoop()
    {
        while (true)
        {
            if (!isPaused && !isInAlert)
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
                            if (room != null && room.patrolPoints != null &&
                                patrolIndex < room.patrolPoints.Length)
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
        if (room != null && room.patrolPoints != null &&
            patrolIndex < room.patrolPoints.Length)
        {
            agent.SetDestination(room.patrolPoints[patrolIndex].position);
        }
    }

    bool ReachedDestination()
    {
        return !agent.pathPending && agent.remainingDistance <= pointReachedThreshold;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPaused && !isInAlert)
        {
            isPaused = true;
            pauseTimer = pauseDuration;
            agent.isStopped = true;
            
            string direction = GetCurrentDirectionName();
            SetAnimationForDirection("idle", direction);
        }
    }

    public void ReactToStolenItem()
    {
        if (!isInAlert && alertRoutine == null)
        {
            alertRoutine = StartCoroutine(AlarmThenRunRoutine());
        }
        var dialog = Instantiate(alarmDialog, transform.position + Vector3.up*2.85f, Quaternion.identity);
        Destroy(dialog, 2f); // Destroy after 1 second

    }

    private IEnumerator AlarmThenRunRoutine()
    {
        isInAlert = true;
        agent.isStopped = true;

        string direction = GetCurrentDirectionName();

        SetAnimationForDirection("alarmed", direction);
        yield return new WaitForSeconds(2f);

        float originalSpeed = agent.speed;
        agent.speed *= 1.5f;
        agent.isStopped = false;
        SetAnimationForDirection("run", direction);
        yield return new WaitForSeconds(5f);

        agent.speed = originalSpeed;
        isInAlert = false;
        alertRoutine = null;
    }

    private void SetAnimationForDirection(string action, string direction)
    {
        string animName = $"{action} {direction}";

        if (direction == "front")
            SetAnimation(frontAnim, animName);
        else if (direction == "back")
            SetAnimation(backAnim, animName);
        else
            SetAnimation(sideAnim, animName);
    }

    private string GetCurrentDirectionName()
    {
        if (frontModel.activeSelf)
            return "front";
        if (backModel.activeSelf)
            return "back";
        return "side";
    }
}