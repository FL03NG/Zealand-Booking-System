using System.Collections.Generic;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;

namespace Zealand_Booking_System_Library.Service
{
    /// <summary>
    /// Handles user-related business logic such as authentication and user management.
    /// The service ensures security-related concerns, like password handling,
    /// are dealt with before data reaches the repository layer.
    /// </summary>
    public class UserService
    {
        /// <summary>
        /// Repository dependency injected to separate business rules
        /// from persistence logic.
        /// </summary>
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }
        /// <summary>
        /// Authenticates a user by verifying credentials.
        /// Password verification happens here to avoid exposing
        /// sensitive logic to other layers.
        /// </summary>
        public Account Login(string username, string password)
        {
            Account user = _repo.GetByUsername(username);
            if (user == null)
                return null;

            // Password verification is handled here so only validated users
            // ever get returned to the calling layer.
            if (PasswordHasher.Verify(user.PasswordHash, password))
                return user;

            return null;
        }
        /// <summary>
        /// Creates a new user while ensuring the password is safely hashed
        /// before it is stored, keeping security concerns centralized.
        /// </summary>
        public void Create(Account user, string role)
        {
            user.PasswordHash = PasswordHasher.Hash(user.PasswordHash);

            _repo.CreateUser(user, role);
        }
        /// <summary>
        /// Updates user information while keeping persistence logic
        /// inside the repository.
        /// </summary>
        public void UpdateUser(Account user)
        {
            _repo.UpdateUser(user);
        }
        /// <summary>
        /// Removes a user from the system through the repository,
        /// so deletion logic stays consistent and centralized.
        /// </summary>
        public void DeleteUser(int id)
        {
            _repo.DeleteUser(id);
        }
        /// <summary>
        /// Retrieves a single user by ID, relying on the repository
        /// for data access and structure.
        /// </summary>
        public Account GetById(int id)
        {
            return _repo.GetById(id);
        }
        /// <summary>
        /// Returns all users, keeping the service focused on coordination
        /// rather than data retrieval details.
        /// </summary>
        public List<Account> GetAllUsers()
        {
            return _repo.GetAllUsers();
        }

    }
}