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
        public string Name { get; set; }
        public string Size {  get; set; }
        public string Description { get; set; }
        public string Location { get; set; }

        public Room() { }
        public Room(string name, string size, string description, string location)
        {
            Name = name;
            Size = size;
            Description = description;
            Location = location;
        }
    }
}
