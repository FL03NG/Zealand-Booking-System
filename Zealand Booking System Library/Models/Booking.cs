using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Booking_System_Library.Models
{
    public class Booking
    {
        public int BookingID {  get; set; }
        public int RoomID { get; set; }
        public Room Room { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
        public DateTime Date {  get; set; }
        public TimeSlot TimeSlot { get; set; }


    }
}
