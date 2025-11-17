using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        // Opret ny bruger
        public void AddUser(Account account)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Account (Username, PasswordHash) " +
                    "VALUES (@Username, @PasswordHash)", conn);

                cmd.Parameters.AddWithValue("@Username", account.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", account.PasswordHash);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Hent alle brugere
        public List<Account> GetAllUsers()
        {
            List<Account> users = new List<Account>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT AccountID, Username, PasswordHash FROM Account",
                    conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Account a = new Account();
                    a.AccountID = (int)reader["AccountID"];
                    a.Username = (string)reader["Username"];
                    a.PasswordHash = (string)reader["PasswordHash"];

                    users.Add(a);
                }
            }

            return users;
        }

        // Opdater bruger
        public void UpdateUser(Account account)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Account " +
                    "SET Username = @Username, PasswordHash = @PasswordHash " +
                    "WHERE AccountID = @AccountID", conn);

                cmd.Parameters.AddWithValue("@AccountID", account.AccountID);
                cmd.Parameters.AddWithValue("@Username", account.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", account.PasswordHash);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Slet bruger
        public void DeleteUser(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Account WHERE AccountID = @AccountID", conn);

                cmd.Parameters.AddWithValue("@AccountID", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}

