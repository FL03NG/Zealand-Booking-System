using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Booking_System_Library.Models
{
    public enum TimeSlot
    {
        [Display(Name = "08:00 - 10:00")]
        Slot08_10,
        [Display(Name = "10:00 - 12:00")]
        Slot10_12,
        [Display(Name = "12:00 - 14:00")]
        Slot12_14,
        [Display(Name = "14:00 - 16:00")]
        Slot14_16
    }
    public class Booking
    {
        public int BookingID {  get; set; }
        public string BookingDescription { get; set; }
        public int RoomID { get; set; }
        public Room Room { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
        public DateTime Date {  get; set; }
        public TimeSlot TimeSlot { get; set; }

        public Booking() { }
    }
}
