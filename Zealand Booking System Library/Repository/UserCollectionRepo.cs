using Microsoft.Data.SqlClient;
using System;
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

        // Hent alle brugere (til din UserList-side)
        public List<Account> GetAllUsers()
        {
            List<Account> users = new List<Account>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = "SELECT AccountID, Username, PasswordHash, AccountRole FROM Account";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Account user = MapToAccount(reader);
                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }

        // Hent én bruger på id
        public Account GetById(int accountId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql =
                    "SELECT AccountID, Username, PasswordHash, AccountRole " +
                    "FROM Account " +
                    "WHERE AccountID = @id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", accountId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }

                        Account account = MapToAccount(reader);
                        return account;
                    }
                }
            }
        }

        // Login på username + password
        public Account Login(string username, string passwordHash)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql =
                    "SELECT AccountID, Username, PasswordHash, AccountRole " +
                    "FROM Account " +
                    "WHERE Username = @username";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }

                        string storedHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
                        if (storedHash != passwordHash)
                        {
                            return null;
                        }

                        Account account = MapToAccount(reader);
                        return account;
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

        // Bruges fx til en generel liste
        public List<Account> GetAll()
        {
            List<Account> accounts = new List<Account>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql =
                    "SELECT AccountID, Username, PasswordHash, AccountRole " +
                    "FROM Account";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Account account = MapToAccount(reader);
                            accounts.Add(account);
                        }
                    }
                }
            }

            return accounts;
        }

        // Fælles mapping fra SqlDataReader → Account/subklasse
        private Account MapToAccount(SqlDataReader reader)
        {
            string role = reader.GetString(reader.GetOrdinal("AccountRole"));

            Account account;

            if (role == "Administrator")
            {
                account = new Administrator();
            }
            else if (role == "Teacher")
            {
                account = new Teacher();
            }
            else if (role == "Student")
            {
                account = new Student();
            }
            else
            {
                account = new Account();
            }

            account.AccountID = reader.GetInt32(reader.GetOrdinal("AccountID"));
            account.Username = reader.GetString(reader.GetOrdinal("Username"));
            account.PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
            account.Role = role;

            return account;
        }
    }
}
