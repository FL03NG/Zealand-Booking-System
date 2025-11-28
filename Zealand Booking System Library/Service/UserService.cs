using System.Collections.Generic;
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
            Account user = _repo.GetByUsername(username);
            if (user == null)
                return null;

            // Argon2 verify
            if (PasswordHasher.Verify(user.PasswordHash, password))
                return user;

            return null;
        }

        public void Create(Account user, string role)
        {
            // Hash password inden det sendes til repository
            user.PasswordHash = PasswordHasher.Hash(user.PasswordHash);

            _repo.CreateUser(user, role);
        }

        public void UpdateUser(Account user)
        {
            _repo.UpdateUser(user);
        }

        public void DeleteUser(int id)
        {
            _repo.DeleteUser(id);
        }

        public Account GetById(int id)
        {
            return _repo.GetById(id);
        }

        public List<Account> GetAllUsers()
        {
            return _repo.GetAllUsers();
        }

    }
}