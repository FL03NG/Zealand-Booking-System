using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Zealand_Booking_System_Library.Models;

namespace Zealand_Booking_System_Library.Repository
{
    public class UserCollectionRepo : IUserRepository
    {
        private readonly string _connectionString;

        public UserCollectionRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Hent alle brugere
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

        // Hent én bruger
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

        // Login (simplet)
        public Account Login(string username, string passwordHash)
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

                        string storedHash = reader.GetString(reader.GetOrdinal("PasswordHash"));

                        if (storedHash != passwordHash)
                            return null;

                        return MapToAccount(reader);
                    }
                }
            }
        }

        // Opret ny bruger
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

        public void DeleteUser(int accountId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // 1) Slet alle bookinger for brugeren
                string deleteBookingsSql = "DELETE FROM Booking WHERE AccountID = @id";
                using (SqlCommand deleteBookingsCmd = new SqlCommand(deleteBookingsSql, conn))
                {
                    deleteBookingsCmd.Parameters.AddWithValue("@id", accountId);
                    deleteBookingsCmd.ExecuteNonQuery();
                }

                // 2) Slet fra Administrator, Teacher, Student
                string deleteAdminSql = "DELETE FROM Administrator WHERE AdministratorID = @id";
                using (SqlCommand deleteAdminCmd = new SqlCommand(deleteAdminSql, conn))
                {
                    deleteAdminCmd.Parameters.AddWithValue("@id", accountId);
                    deleteAdminCmd.ExecuteNonQuery();
                }

                string deleteTeacherSql = "DELETE FROM Teacher WHERE TeacherID = @id";
                using (SqlCommand deleteTeacherCmd = new SqlCommand(deleteTeacherSql, conn))
                {
                    deleteTeacherCmd.Parameters.AddWithValue("@id", accountId);
                    deleteTeacherCmd.ExecuteNonQuery();
                }

                string deleteStudentSql = "DELETE FROM Student WHERE StudentID = @id";
                using (SqlCommand deleteStudentCmd = new SqlCommand(deleteStudentSql, conn))
                {
                    deleteStudentCmd.Parameters.AddWithValue("@id", accountId);
                    deleteStudentCmd.ExecuteNonQuery();
                }

                // 3) Slet selve brugeren
                string deleteAccountSql = "DELETE FROM Account WHERE AccountID = @id";
                using (SqlCommand deleteAccountCmd = new SqlCommand(deleteAccountSql, conn))
                {
                    deleteAccountCmd.Parameters.AddWithValue("@id", accountId);
                    deleteAccountCmd.ExecuteNonQuery();
                }
            }
        }


        // Alternativ hent alle
        public List<Account> GetAll()
        {
            List<Account> accounts = new List<Account>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql =
                    "SELECT AccountID, Username, PasswordHash, AccountRole FROM Account";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        accounts.Add(MapToAccount(reader));
                    }
                }
            }

            return accounts;
        }

        // Map SQL -> Account objekt
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
        public Account GetUserById(int id)
        {
            return GetById(id);
        }
    }
}