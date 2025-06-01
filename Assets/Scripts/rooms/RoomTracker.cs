using UnityEngine;

public class RoomTracker : MonoBehaviour
{
    public Room currentRoom;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Room room = other.GetComponent<Room>();
        if (room != null)
        {
            currentRoom = room;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Room>() == currentRoom)
        {
            currentRoom = null;
        }
    }
}
