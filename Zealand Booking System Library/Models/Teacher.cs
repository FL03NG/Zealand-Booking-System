using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Booking_System_Library.Models
{
    public class Teacher : Account
    {
        public int TeacherID { get; set; }
        public override string Role { get; set; } = "Teacher";

        public Teacher() : base() { }

        public Teacher(int accountID, string username, string passwordHash)
            : base(accountID, username, passwordHash)
        {
        }
    }
}
