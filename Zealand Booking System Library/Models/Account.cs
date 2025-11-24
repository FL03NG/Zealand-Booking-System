using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Booking_System_Library.Models
{
    public class Account
    {
        public int AccountID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public virtual string Role { get; set; } = "Account";

        public Account() { }
        public Account(int accountID, string username, string passwordHash)
        {
            AccountID = accountID;
            Username = username;
            PasswordHash = passwordHash;
        }
    }
}
