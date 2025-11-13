using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zealand_Booking_System_Library.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public User() { }
        public User(int userID, string username, string passwordHash)
        {
            UserID = userID;
            Username = username;
            PasswordHash = passwordHash;
        }
    }
}
