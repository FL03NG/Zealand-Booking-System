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
                            var role = reader.GetString(reader.GetOrdinal("AccountRole"));

                            Account user = role switch
                            {
                                "Administrator" => new Administrator(),
                                "Teacher" => new Teacher(),
                                "Student" => new Student(),
                                _ => new Account()
                            };

                            user.AccountID = reader.GetInt32(reader.GetOrdinal("AccountID"));
                            user.Username = reader.GetString(reader.GetOrdinal("Username"));
                            user.PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
                            user.Role = role;

                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }
        public Account GetById(int accountId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
            SELECT 
                a.AccountID,
                a.Username,
                a.PasswordHash,
                CASE 
                    WHEN ad.AdministratorID IS NOT NULL THEN 'Administrator'
                    WHEN t.TeacherID IS NOT NULL THEN 'Teacher'
                    WHEN s.StudentID IS NOT NULL THEN 'Student'
                    ELSE 'Account'
                END AS AccountRole
            FROM Account a
            LEFT JOIN Administrator ad ON a.AccountID = ad.AdministratorID
            LEFT JOIN Teacher t ON a.AccountID = t.TeacherID
            LEFT JOIN Student s ON a.AccountID = s.StudentID
            WHERE a.AccountID = @id";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", accountId);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read()) return null;

            return MapToAccount(reader);
        }

        public Account Login(string username, string passwordHash)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
            SELECT 
                a.AccountID,
                a.Username,
                a.PasswordHash,
                CASE 
                    WHEN ad.AdministratorID IS NOT NULL THEN 'Administrator'
                    WHEN t.TeacherID IS NOT NULL THEN 'Teacher'
                    WHEN s.StudentID IS NOT NULL THEN 'Student'
                END AS AccountRole
            FROM Account a
            LEFT JOIN Administrator ad ON a.AccountID = ad.AdministratorID
            LEFT JOIN Teacher t ON a.AccountID = t.TeacherID
            LEFT JOIN Student s ON a.AccountID = s.StudentID
            WHERE a.Username = @username";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", username);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read()) return null;

            if ((string)reader["PasswordHash"] != passwordHash)
                return null;

            return MapToAccount(reader);
        }

        public void CreateUser(Account user, string role)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            // Insert into Account
            string sql = @"
            INSERT INTO Account (Username, PasswordHash)
            OUTPUT INSERTED.AccountID
            VALUES (@u, @p)";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", user.Username);
            cmd.Parameters.AddWithValue("@p", user.PasswordHash);

            int accountId = (int)cmd.ExecuteScalar();

            // Insert into role table
            string roleSql = role switch
            {
                "Administrator" => "INSERT INTO Administrator (AdministratorID) VALUES (@id)",
                "Teacher" => "INSERT INTO Teacher (TeacherID) VALUES (@id)",
                "Student" => "INSERT INTO Student (StudentID) VALUES (@id)",
                _ => null
            };

            if (roleSql != null)
            {
                using var roleCmd = new SqlCommand(roleSql, conn);
                roleCmd.Parameters.AddWithValue("@id", accountId);
                roleCmd.ExecuteNonQuery();
            }
        }

        public List<Account> GetAll()
        {
            List<Account> accounts = new List<Account>();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
            SELECT 
                a.AccountID,
                a.Username,
                a.PasswordHash,
                CASE 
                    WHEN ad.AdministratorID IS NOT NULL THEN 'Administrator'
                    WHEN t.TeacherID IS NOT NULL THEN 'Teacher'
                    WHEN s.StudentID IS NOT NULL THEN 'Student'
                    ELSE 'Account'
                END AS AccountRole
            FROM Account a
            LEFT JOIN Administrator ad ON a.AccountID = ad.AdministratorID
            LEFT JOIN Teacher t ON a.AccountID = t.TeacherID
            LEFT JOIN Student s ON a.AccountID = s.StudentID";

            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                accounts.Add(MapToAccount(reader));
            }

            return accounts;
        }

        private Account MapToAccount(SqlDataReader reader)
        {
            string role = reader["AccountRole"].ToString();

            Account acc = role switch
            {
                "Administrator" => new Administrator(),
                "Teacher" => new Teacher(),
                "Student" => new Student(),
                _ => new Account(),
            };

            acc.AccountID = (int)reader["AccountID"];
            acc.Username = reader["Username"].ToString();
            acc.PasswordHash = reader["PasswordHash"].ToString();

            return acc;
        }
    }
}

