using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Booking_System_Library.Models
{
    public enum RoomType
    {
        ClassRoom = 1,
        MeetingRoom = 2
    }

    public class Room
    {
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public string RoomDescription { get; set; }
        public string RoomLocation { get; set; }
        public RoomType RoomType { get; set; }
        public bool HasSmartBoard { get; set; }
    

        public Room() { }
        public Room(string roomName, string roomDescription, string roomLocation, RoomType roomType, bool hasSmartBoard)
        {
            RoomName = roomName;
            RoomDescription = roomDescription;
            RoomLocation = roomLocation;
            RoomType = roomType;
            HasSmartBoard = hasSmartBoard;
        }
    }
}
