using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Booking_System_Library.Models
{
    public class Administrator : Account
    {
        public string AdministratorID { get; set; }
        public override string Role { get; set; } = "Administrator";

        public Administrator() : base() { }

        public Administrator(int accountID, string username, string passwordHash)
            : base(accountID, username, passwordHash)
        {
        }
    }
}
