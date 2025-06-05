using UnityEngine;
using UnityEngine.AI;

using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class PatrolGuard : MonoBehaviour
{
    public List<string> roomNames; 
    public float reachDistance = 0.2f;

    public GameObject frontModel;
    public GameObject backModel;
    public GameObject sideModel;
    
    private List<Transform> waypoints = new List<Transform>();
    private NavMeshAgent agent;
    private int index = 0;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Start()
    {
        LoadWaypointsFromRooms();
        if (waypoints.Count > 0)
            agent.SetDestination(waypoints[0].position);
    }

    void Update()
    {
        if (waypoints.Count == 0 || agent.pathPending) return;

        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            UpdateModelDirection(agent.velocity);
        }

        if (agent.remainingDistance <= reachDistance)
        {
            index = (index + 1) % waypoints.Count;
            agent.SetDestination(waypoints[index].position);
        }
    }
    
    void UpdateModelDirection(Vector3 velocity)
    {
        if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
        {
            ShowOnlyModel(sideModel);
            // sideModel.transform.localScale = new Vector3(
            //     velocity.x > 0 ? 1 : -1, 1, 1
            // );
            sideModel.transform.localScale = new Vector3(
                velocity.x > 0 ? -1 : 1, 1, 1
            );
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
}
