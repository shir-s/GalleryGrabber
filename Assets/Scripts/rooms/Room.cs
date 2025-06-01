using UnityEngine;

public class Room : MonoBehaviour
{
    public string roomName;
    public Transform[] patrolPoints;
    
    private void Awake()
    {
        if (string.IsNullOrEmpty(roomName))
            roomName = gameObject.name;

        patrolPoints = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            patrolPoints[i] = transform.GetChild(i);
        }
    }

}
