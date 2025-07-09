using System.Collections;
using System.Collections.Generic;
using Managers;
using Sound;
using Spine.Unity;
using UnityEngine;
using UnityEngine.AI;

namespace Guards
{
    public class PatrolGuard : MonoBehaviour
    {
        [Header("Patrol Settings")]
        public List<string> roomNames;
        public float reachDistance = 0.2f;
        public float pauseDuration = 2f;

        [Header("Models")]
        public GameObject frontModel;
        public GameObject backModel;
        public GameObject sideModel;

        [Header("Dialog & Audio")]
        [SerializeField] private GameObject alarmDialog;

        private SkeletonAnimation _frontAnim;
        private SkeletonAnimation _backAnim;
        private SkeletonAnimation _sideAnim;

        private readonly List<Transform> _waypoints = new();
        private NavMeshAgent _agent;
        private int _currentWaypointIndex = 0;

        private bool _isPaused = false;
        private float _pauseTimeRemaining = 0f;

        private float _stepTimer = 0f;
        private const float StepInterval = 0.7f;
        private Transform _playerTransform;
        private const float MaxStepVolumeDistance = 20f;
        private const float MaxStepVolume = 0.3f;

        private bool _isInAlert = false;
        private Coroutine _alertRoutine = null;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;

            if (frontModel) _frontAnim = frontModel.GetComponent<SkeletonAnimation>();
            if (backModel) _backAnim = backModel.GetComponent<SkeletonAnimation>();
            if (sideModel) _sideAnim = sideModel.GetComponent<SkeletonAnimation>();
        }

        private void Start()
        {
            LoadWaypointsFromRooms();
            _playerTransform = GameObject.FindWithTag("Player")?.transform;

            if (_waypoints.Count > 0)
                _agent.SetDestination(_waypoints[0].position);
        }

        private void Update()
        {
            if (_isPaused)
            {
                _pauseTimeRemaining -= Time.deltaTime;
                if (_pauseTimeRemaining <= 0f)
                {
                    _isPaused = false;
                    _agent.isStopped = false;
                    _agent.SetDestination(_waypoints[_currentWaypointIndex].position);
                }

                SetIdleAnimation();
                return;
            }

            if (_waypoints.Count == 0 || _agent.pathPending)
                return;

            Vector3 velocity = _agent.velocity;
            bool isMoving = velocity.sqrMagnitude > 0.01f;

            if (isMoving)
            {
                UpdateModelDirection(velocity);
            }
            else
            {
                _stepTimer = 0f;
            }

            SetWalkOrIdleAnimation(isMoving);

            if (_agent.remainingDistance <= reachDistance)
            {
                _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Count;
                _agent.SetDestination(_waypoints[_currentWaypointIndex].position);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !_isPaused)
            {
                _isPaused = true;
                _pauseTimeRemaining = pauseDuration;
                _agent.isStopped = true;
            }
        }

        private void SetIdleAnimation()
        {
            if (sideModel.activeSelf) SetAnimation(_sideAnim, "idle side");
            else if (backModel.activeSelf) SetAnimation(_backAnim, "idle back");
            else if (frontModel.activeSelf) SetAnimation(_frontAnim, "idle front");
        }

        private void SetWalkOrIdleAnimation(bool isMoving)
        {
            string action = isMoving ? "walking" : "idle";

            if (sideModel.activeSelf) SetAnimation(_sideAnim, $"{action} side");
            else if (backModel.activeSelf) SetAnimation(_backAnim, $"{action} back");
            else if (frontModel.activeSelf) SetAnimation(_frontAnim, $"{action} front");
        }

        private void UpdateModelDirection(Vector3 velocity)
        {
            if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
            {
                ShowOnlyModel(sideModel);
                sideModel.transform.localScale = new Vector3(Mathf.Sign(velocity.x), 1f, 1f);
            }
            else
            {
                ShowOnlyModel(velocity.y > 0 ? backModel : frontModel);
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
            if (frontModel) frontModel.SetActive(modelToShow == frontModel);
            if (backModel) backModel.SetActive(modelToShow == backModel);
            if (sideModel) sideModel.SetActive(modelToShow == sideModel);
        }

        private void LoadWaypointsFromRooms()
        {
            foreach (var roomName in roomNames)
            {
                Room room = RoomManager.instance.GetRoomByName(roomName);
                if (room?.patrolPoints == null || room.patrolPoints.Length == 0)
                {
                    Debug.LogWarning($"Room '{roomName}' not found or has no patrol points.");
                    continue;
                }

                _waypoints.AddRange(room.patrolPoints);
            }
        }

        public void ReactToStolenItem()
        {
            if (!_isInAlert && _alertRoutine == null)
            {
                _alertRoutine = StartCoroutine(AlarmThenRunRoutine());
            }

            SoundManager.Instance.PlaySound("GuardGasp", transform);
            var dialog = Instantiate(alarmDialog, transform.position + Vector3.up * 2.85f, Quaternion.identity);
            Destroy(dialog, 2f);
        }

        private IEnumerator AlarmThenRunRoutine()
        {
            _isInAlert = true;
            _agent.isStopped = true;

            string direction = GetCurrentDirectionName();
            SetAnimationForDirection("alarmed", direction);

            yield return new WaitForSeconds(2f);

            float originalSpeed = _agent.speed;
            _agent.speed *= 1.5f;
            _agent.isStopped = false;

            SetAnimationForDirection("run", direction);
            yield return new WaitForSeconds(5f);

            _agent.speed = originalSpeed;
            _isInAlert = false;
            _alertRoutine = null;
        }

        private void SetAnimationForDirection(string action, string direction)
        {
            string animName = $"{action} {direction}";

            switch (direction)
            {
                case "front": SetAnimation(_frontAnim, animName); break;
                case "back": SetAnimation(_backAnim, animName); break;
                default: SetAnimation(_sideAnim, animName); break;
            }
        }

        private string GetCurrentDirectionName()
        {
            if (frontModel && frontModel.activeSelf) return "front";
            if (backModel && backModel.activeSelf) return "back";
            return "side";
        }
    }
}
