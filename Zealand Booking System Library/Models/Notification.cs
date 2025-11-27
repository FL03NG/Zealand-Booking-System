using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Booking_System_Library.Models
{
    //Bruges til at sende en notifikation til brugeren om at deres booking er blevet slettet
    public class Notification
    {
        public int NotificationID { get; set; }
        public int AccountID { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public Account Account { get; set; }
    }
}
