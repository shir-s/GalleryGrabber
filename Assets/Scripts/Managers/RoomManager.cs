using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Managers
{
    public class RoomManager : MonoBehaviour
    {
        public static RoomManager instance;

        public List<Room> allRooms = new List<Room>();

        private void Awake()
        {
            instance = this;
            allRooms = FindObjectsOfType<Room>().ToList();
        }

        public Room GetRandomRoom()
        {
            return allRooms[Random.Range(0, allRooms.Count)];
        }
    
        public Room GetRoomByName(string name)
        {
            return allRooms.FirstOrDefault(r => r.roomName == name);
        }

    }
}
