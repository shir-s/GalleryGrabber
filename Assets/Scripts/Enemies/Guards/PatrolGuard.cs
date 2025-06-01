using UnityEngine;
using UnityEngine.AI;

using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class PatrolGuard : MonoBehaviour
{
    public List<string> roomNames; // שמות החדרים לפי הסדר
    public float reachDistance = 0.2f;

    private List<Transform> waypoints = new List<Transform>();
    private NavMeshAgent agent;
    private int index = 0;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        LoadWaypointsFromRooms();
    }

    void Start()
    {
        if (waypoints.Count > 0)
            agent.SetDestination(waypoints[0].position);
    }

    void Update()
    {
        if (waypoints.Count == 0) return;
        if (agent.pathPending)    return;

        if (agent.remainingDistance <= reachDistance)
        {
            index = (index + 1) % waypoints.Count;
            agent.SetDestination(waypoints[index].position);
        }
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
