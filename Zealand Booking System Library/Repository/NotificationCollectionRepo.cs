using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Zealand_Booking_System_Library.Models;

namespace Zealand_Booking_System_Library.Repository
{
    public class NotificationCollectionRepo : INotificationRepository
    {
        private readonly string _connectionString;

        public NotificationCollectionRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Add(Notification notification)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"INSERT INTO Notifications (AccountID, Message) 
                           VALUES (@AccountID, @Message)";

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@AccountID", notification.AccountID);
            cmd.Parameters.AddWithValue("@Message", notification.Message);

            cmd.ExecuteNonQuery();
        }

        public List<Notification> GetUnread(int accountId)
        {
            List<Notification> list = new();

            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"SELECT * FROM Notifications
                           WHERE AccountID = @AccountID AND IsRead = 0";

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@AccountID", accountId);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Notification
                {
                    NotificationID = reader.GetInt32(0),
                    AccountID = reader.GetInt32(1),
                    Message = reader.GetString(2),
                    IsRead = reader.GetBoolean(3),
                    CreatedAt = reader.GetDateTime(4)
                });
            }

            return list;
        }

        public void MarkAsRead(int notificationId)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"UPDATE Notifications SET IsRead = 1 
                           WHERE NotificationID = @ID";

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ID", notificationId);

            cmd.ExecuteNonQuery();
        }
    }
}
