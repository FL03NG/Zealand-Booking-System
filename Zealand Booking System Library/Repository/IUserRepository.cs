using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Models;

namespace Zealand_Booking_System_Library.Repository
{
    public interface IUserRepository
    {
        public void AddUser(Account user);
        public void DeleteUser(int id);
        public List<Account> GetAllUsers();
        public void UpdateUser(Account user);
    }
}
