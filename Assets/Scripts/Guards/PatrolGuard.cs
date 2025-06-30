using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Sound;
using Spine.Unity;

public class PatrolGuard : MonoBehaviour
{
    public List<string> roomNames;
    public float reachDistance = 0.2f;

    public GameObject frontModel;
    public GameObject backModel;
    public GameObject sideModel;

    [SerializeField] private GameObject alarmDialog;
    private SkeletonAnimation frontAnim;
    private SkeletonAnimation backAnim;
    private SkeletonAnimation sideAnim;

    private List<Transform> waypoints = new List<Transform>();
    private NavMeshAgent agent;
    private int index = 0;

    private bool isPaused = false;
    private float pauseTimer = 0f;
    public float pauseDuration = 2f;
    
    private float stepTimer = 0f;
    private float stepInterval = 0.7f;
    private Transform playerTransform;
    private float maxStepVolumeDistance = 20f;
    private float maxStepVolume = 0.3f;
    
    private bool isInAlert = false;
    private Coroutine alertRoutine = null;
    

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (frontModel != null) frontAnim = frontModel.GetComponent<SkeletonAnimation>();
        if (backModel != null) backAnim = backModel.GetComponent<SkeletonAnimation>();
        if (sideModel != null) sideAnim = sideModel.GetComponent<SkeletonAnimation>();
    }

    private void Start()
    {
        LoadWaypointsFromRooms();
        playerTransform = GameObject.FindWithTag("Player")?.transform;

        //SoundManager.Instance.PlaySound("Guard", transform);
        if (waypoints.Count > 0)
            agent.SetDestination(waypoints[0].position);
    }

    private void Update()
    {
        if (isPaused)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                isPaused = false;
                agent.isStopped = false;
                agent.SetDestination(waypoints[index].position);
            }

            SetIdleAnimation(); // keep idle animation while paused
            return;
        }

        if (waypoints.Count == 0 || agent.pathPending) return;

        Vector3 velocity = agent.velocity;
        bool isMoving = velocity.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            UpdateModelDirection(velocity);
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f && playerTransform != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
                float volume = Mathf.Clamp01(1f - (distanceToPlayer / maxStepVolumeDistance))* maxStepVolume;
                SoundManager.Instance.PlaySound("Guard", transform, volume);
                stepTimer = stepInterval / agent.speed;
            }
        }
        else
        {
            stepTimer = 0f;
        }

        if (sideModel.activeSelf)
        {
            SetAnimation(sideAnim, isMoving ? "walking side" : "idle side");
        }
        else if (backModel.activeSelf)
        {
            SetAnimation(backAnim, isMoving ? "walking back" : "idle back");
        }
        else if (frontModel.activeSelf)
        {
            SetAnimation(frontAnim, isMoving ? "walking front" : "idle front");
        }

        if (agent.remainingDistance <= reachDistance)
        {
            index = (index + 1) % waypoints.Count;
            agent.SetDestination(waypoints[index].position);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPaused)
        {
            isPaused = true;
            pauseTimer = pauseDuration;
            agent.isStopped = true;
        }
    }

    private void SetIdleAnimation()
    {
        if (sideModel.activeSelf)
            SetAnimation(sideAnim, "idle side");
        else if (backModel.activeSelf)
            SetAnimation(backAnim, "idle back");
        else if (frontModel.activeSelf)
            SetAnimation(frontAnim, "idle front");
    }

    private void UpdateModelDirection(Vector3 velocity)
    {
        if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
        {
            ShowOnlyModel(sideModel);
            if (velocity.x > 0)
                sideModel.transform.localScale = new Vector3(1, 1, 1); // right
            else
                sideModel.transform.localScale = new Vector3(-1, 1, 1); // left
        }
        else
        {
            if (velocity.y > 0)
                ShowOnlyModel(backModel);
            else
                ShowOnlyModel(frontModel);
        }
    }

    private void SetAnimation(SkeletonAnimation anim, string animationName)
    {
        if (anim != null && anim.AnimationName != animationName)
        {
            anim.AnimationName = animationName;
        }
    }

    private void ShowOnlyModel(GameObject modelToShow)
    {
        if (frontModel != null) frontModel.SetActive(modelToShow == frontModel);
        if (backModel != null) backModel.SetActive(modelToShow == backModel);
        if (sideModel != null) sideModel.SetActive(modelToShow == sideModel);
    }

    private void LoadWaypointsFromRooms()
    {
        foreach (string name in roomNames)
        {
            Room room = RoomManager.instance.GetRoomByName(name);
            if (room != null && room.patrolPoints != null && room.patrolPoints.Length > 0)
            {
                foreach (Transform point in room.patrolPoints)
                {
                    waypoints.Add(point);
                }
            }
            else
            {
                Debug.LogWarning($"Room '{name}' not found or has no patrol points.");
            }
        }
    }
    
    public void ReactToStolenItem()
    {
        if (!isInAlert && alertRoutine == null)
        {
            // alertRoutine = StartCoroutine(AlarmThenResumeRoutine());
            alertRoutine = StartCoroutine(AlarmThenRunRoutine());
        }
        var dialog = Instantiate(alarmDialog, transform.position + Vector3.up *2.85f, Quaternion.identity);
        Destroy(dialog, 2f); // Destroy after 1 second
    }


    // private IEnumerator AlarmThenResumeRoutine()
    // {
    //     isInAlert = true;
    //     agent.isStopped = true;
    //
    //     string direction = GetCurrentDirectionName();
    //     SetAnimationForDirection("alarmed", direction);
    //
    //     yield return new WaitForSeconds(2f);
    //
    //     agent.isStopped = false;
    //     isInAlert = false;
    //     alertRoutine = null;
    // }
    
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
        if (frontModel != null && frontModel.activeSelf)
            return "front";
        if (backModel != null && backModel.activeSelf)
            return "back";
        return "side";
    }

}
