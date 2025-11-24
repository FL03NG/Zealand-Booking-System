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
        Account GetById(int accountId);
        Account Login(string username, string passwordHash);
        void CreateUser(Account user, string role);
        List<Account> GetAll();
        List<Account> GetAllUsers();
    }
}
