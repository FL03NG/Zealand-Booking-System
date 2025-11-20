using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Models;
using Microsoft.Data.SqlClient;

namespace Zealand_Booking_System_Library.Repository
{
    public class RoomCollectionRepo : IRoomRepository
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
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    @"UPDATE Room 
              SET RoomName=@RoomName,
                  Size=@Size,
                  RoomDescription=@RoomDescription,
                  RoomLocation=@RoomLocation
              WHERE RoomID=@RoomID", conn);

                cmd.Parameters.AddWithValue("@RoomID", room.RoomID);
                cmd.Parameters.AddWithValue("@RoomName", room.RoomName);
                cmd.Parameters.AddWithValue("@Size", room.Size);
                cmd.Parameters.AddWithValue("@RoomDescription", room.RoomDescription ?? "");
                cmd.Parameters.AddWithValue("@RoomLocation", room.RoomLocation);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void DeleteRoom(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Først: slet alle bookinger for det lokale
                string deleteBookingsSql = "DELETE FROM Booking WHERE RoomID = @id";
                SqlCommand cmdBookings = new SqlCommand(deleteBookingsSql, conn);
                cmdBookings.Parameters.AddWithValue("@id", id);
                cmdBookings.ExecuteNonQuery();

                // Så: slet selve lokalet
                string deleteRoomSql = "DELETE FROM Room WHERE RoomID = @id";
                SqlCommand cmdRoom = new SqlCommand(deleteRoomSql, conn);
                cmdRoom.Parameters.AddWithValue("@id", id);
                cmdRoom.ExecuteNonQuery();
            }
        }
        public Room GetRoomById(int roomID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql = "SELECT RoomID, RoomName, RoomLocation, Size, RoomDescription FROM Room WHERE RoomID = @RoomID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RoomID", roomID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Room
                            {
                                RoomID = (int)reader["RoomID"],
                                RoomName = reader["RoomName"].ToString(),
                                RoomLocation = reader["RoomLocation"].ToString(),
                                Size = (string)reader["Size"],
                                RoomDescription = reader["RoomDescription"].ToString()
                            };
                        }
                    }
                }
            }

            return null; // hvis ingen fundet
        }
    }
}
