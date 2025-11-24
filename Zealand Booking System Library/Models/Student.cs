using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Booking_System_Library.Models
{
    public class Student : Account
    {
        public int StudentID { get; set; }

        public override string Role { get; set; } = "Student";

        public Student() : base() { }

        public Student(int accountID, string username, string passwordHash)
            : base(accountID, username, passwordHash)
        {
        }
    }
}
