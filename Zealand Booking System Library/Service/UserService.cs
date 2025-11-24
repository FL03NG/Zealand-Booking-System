using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;

namespace Zealand_Booking_System_Library.Service
{
    public class UserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public Account Login(string username, string password)
        {
            // Evt. hashing her
            string hash = password;
            return _repo.Login(username, hash);
        }

        public void Create(Account user, string role)
        {
            _repo.CreateUser(user, role);
        }

        public Account GetById(int id)
        {
            return _repo.GetById(id);
        }

        public List<Account> GetAll()
        {
            return _repo.GetAll();
        }
        public List<Account> GetAllUsers()
        {
            return _repo.GetAllUsers();
        }
    }
}
