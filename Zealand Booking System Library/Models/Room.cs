using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Booking_System_Library.Models
{
    public class Room
    {
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public string Size {  get; set; }
        public string RoomDescription { get; set; }
        public string RoomLocation { get; set; }

        public Room() { }
        public Room(string roomName, string size, string roomDescription, string roomLocation)
        {
            RoomName = roomName;
            Size = size;
            RoomDescription = roomDescription;
            RoomLocation = roomLocation;
        }
    }
}
