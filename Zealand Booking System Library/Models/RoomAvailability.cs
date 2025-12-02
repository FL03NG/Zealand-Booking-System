using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace Zealand_Booking_System_Library.Models
{
    public class RoomAvailability
    {
        public Room Room { get; set; }   // det originale lokale

        // Beregnet info
        public int CurrentBookings { get; set; }
        public int MaxBookings { get; set; }

        // UI-info
        public string StatusColor { get; set; }   // "green", "yellow", "red"
        public string StatusText { get; set; }    // fx "Helt ledig", "Delvist booket"

        public RoomAvailability()
        {
        }
    }
}

