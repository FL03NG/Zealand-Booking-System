using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Models;
using Microsoft.Data.SqlClient;

namespace Zealand_Booking_System_Library.Repository
{
    internal class RoomCollectionRepo : IRoomRepository
    {
        private readonly string _connectionString;

        public RoomCollectionRepo(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void AddRoom(Room room)
        {
            using (SqlConnection conn = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=RoomBooking;Trusted_Connection=True;TrustServerCertificate=True;"))
            {
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Room (RoomName, Size, RoomDescription, RoomLocation) " +
                    "VALUES (@RoomName, @Size, @RoomDescription, @RoomLocation)", conn);

                cmd.Parameters.AddWithValue("@RoomName", room.RoomName);
                cmd.Parameters.AddWithValue("@Size", room.Size);
                cmd.Parameters.AddWithValue("@RoomDescription", room.RoomDescription);
                cmd.Parameters.AddWithValue("@RoomLocation", room.RoomLocation);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public List<Room> GetAllRooms()
        {
            List<Room> rooms = new List<Room>();

            using (SqlConnection conn = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=RoomBooking;Trusted_Connection=True;TrustServerCertificate=True;"))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Room", conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Room r = new Room
                        {
                            RoomID = (int)reader["RoomID"],
                            RoomName = (string)reader["RoomName"],
                            Size = (string)reader["Size"],
                            RoomDescription = (string)reader["RoomDescription"],
                            RoomLocation = (string)reader["RoomLocation"],
                        };
                        rooms.Add(r);
                    }
                }
            }
            return rooms;
        }

        public void UpdateRoom(Room room)
        {
            using (SqlConnection conn = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=RoomBooking;Trusted_Connection=True;TrustServerCertificate=True;"))
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Room SET RoomID=@RoomID, RoomName=@RoomName, Size = @Size, RoomDescription=@RoomDescription, RoomLocation=@RoomLocation", conn);

                cmd.Parameters.AddWithValue("@RoomID", room.RoomID);
                cmd.Parameters.AddWithValue("@RoomName", room.RoomName);
                cmd.Parameters.AddWithValue("@Size", room.Size);
                cmd.Parameters.AddWithValue("@RoomDescription", room.RoomDescription);
                cmd.Parameters.AddWithValue("@RoomLocation", room.RoomLocation);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void DeleteRoom(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Room WHERE RoomID=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
