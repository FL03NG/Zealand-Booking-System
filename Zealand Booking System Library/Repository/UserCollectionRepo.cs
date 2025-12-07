using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Zealand_Booking_System_Library.Models;

namespace Zealand_Booking_System_Library.Repository
{
    /// <summary>
    /// Responsible for persisting and retrieving user data from the database.
    /// This repository centralizes all SQL logic related to accounts,
    /// keeping data access separate from business rules and UI concerns.
    /// </summary>
    public class UserCollectionRepo : IUserRepository
    {
        /// <summary>
        /// Stored once to avoid hard-coding connection details
        /// and to make the repository easier to configure and test.
        /// </summary>
        private readonly string _connectionString;

        public UserCollectionRepo(string connectionString)
        {
            _connectionString = connectionString;
        }
        /// <summary>
        /// Retrieves all users from the database.
        /// Mapping is handled in a dedicated method to keep SQL logic
        /// and object creation clearly separated.
        /// </summary>
        public List<Account> GetAllUsers()
        {
            List<Account> users = new List<Account>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = "SELECT AccountID, Username, PasswordHash, AccountRole FROM Account";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(MapToAccount(reader));
                    }
                }
            }

            return users;
        }
        /// <summary>
        /// Retrieves a single user by ID.
        /// Returning null when no user is found allows the service layer
        /// to decide how missing users should be handled.
        /// </summary>
        public Account GetById(int accountId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql =
                    "SELECT AccountID, Username, PasswordHash, AccountRole " +
                    "FROM Account WHERE AccountID = @id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", accountId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        return MapToAccount(reader);
                    }
                }
            }
        }
        /// <summary>
        /// Retrieves a user by username.
        /// This is primarily used for authentication, so password data
        /// must be included even though it is security-sensitive.
        /// </summary>
        public Account GetByUsername(string username)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql =
                    "SELECT AccountID, Username, PasswordHash, AccountRole " +
                    "FROM Account WHERE Username = @username";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        return MapToAccount(reader);
                    }
                }
            }
        }
        /// <summary>
        /// Creates a new user in the database.
        /// The role is stored separately to allow role assignment
        /// without coupling it too tightly to the Account model.
        /// </summary>
        public void CreateUser(Account user, string role)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql =
                    "INSERT INTO Account (Username, PasswordHash, AccountRole) " +
                    "VALUES (@u, @p, @r)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", user.Username);
                    cmd.Parameters.AddWithValue("@p", user.PasswordHash);
                    cmd.Parameters.AddWithValue("@r", role);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Updates basic user information.
        /// Password changes are intentionally excluded to keep
        /// security-sensitive operations isolated.
        /// </summary>
        public void UpdateUser(Account user)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql = "UPDATE Account " +
                             "SET Username = @Username, AccountRole = @AccountRole " +
                             "WHERE AccountID = @AccountID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", user.Username);
                    cmd.Parameters.AddWithValue("@AccountRole", user.Role);
                    cmd.Parameters.AddWithValue("@AccountID", user.AccountID);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Deletes a user and all related data.
        /// Related records are removed first to maintain referential integrity
        /// and avoid foreign key constraint violations.
        /// </summary>
        public void DeleteUser(int accountId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // Related data is deleted explicitly to keep control
                // over deletion order and side effects.
                string deleteNotificationsSql = "DELETE FROM Notifications WHERE AccountID = @id";
                using (SqlCommand deleteNotifCmd = new SqlCommand(deleteNotificationsSql, conn))
                {
                    deleteNotifCmd.Parameters.AddWithValue("@id", accountId);
                    deleteNotifCmd.ExecuteNonQuery();
                }
                string deleteBookingsSql = "DELETE FROM Booking WHERE AccountID = @id";
                using (SqlCommand cmd = new SqlCommand(deleteBookingsSql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", accountId);
                    cmd.ExecuteNonQuery();
                }
                // Role-specific tables are cleared to avoid orphaned role records.
                string deleteAdminSql = "DELETE FROM Administrator WHERE AdministratorID = @id";
                using (SqlCommand cmd = new SqlCommand(deleteAdminSql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", accountId);
                    cmd.ExecuteNonQuery();
                }
                string deleteTeacherSql = "DELETE FROM Teacher WHERE TeacherID = @id";
                using (SqlCommand cmd = new SqlCommand(deleteTeacherSql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", accountId);
                    cmd.ExecuteNonQuery();
                }
                string deleteStudentSql = "DELETE FROM Student WHERE StudentID = @id";
                using (SqlCommand cmd = new SqlCommand(deleteStudentSql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", accountId);
                    cmd.ExecuteNonQuery();
                }
                // Finally, the account itself is deleted once dependencies are removed.
                string deleteAccountSql = "DELETE FROM Account WHERE AccountID = @id";
                using (SqlCommand cmd = new SqlCommand(deleteAccountSql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", accountId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Converts raw database data into a domain object.
        /// Role-based instantiation is handled here so the rest of the system
        /// can work with proper polymorphic objects.
        /// </summary>
        private Account MapToAccount(SqlDataReader reader)
        {
            string role = reader.GetString(reader.GetOrdinal("AccountRole"));
            Account account = role switch
            {
                "Administrator" => new Administrator(),
                "Teacher" => new Teacher(),
                "Student" => new Student(),
                _ => new Account()
            };
            account.AccountID = reader.GetInt32(reader.GetOrdinal("AccountID"));
            account.Username = reader.GetString(reader.GetOrdinal("Username"));
            account.PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
            account.Role = role;
            return account;
        }
    }
}