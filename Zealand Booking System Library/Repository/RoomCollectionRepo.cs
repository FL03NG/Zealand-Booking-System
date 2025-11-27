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

        // Hent alle lokaler
        public List<Room> GetAllRooms()
        {
            List<Room> rooms = new();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT RoomID, RoomName, Size, RoomDescription, RoomLocation FROM Room";
                SqlCommand cmd = new SqlCommand(sql, conn);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rooms.Add(new Room
                        {
                            RoomID = reader.GetInt32(reader.GetOrdinal("RoomID")),
                            RoomName = reader.GetString(reader.GetOrdinal("RoomName")),
                            Size = reader.GetString(reader.GetOrdinal("Size")),
                            RoomDescription = reader["RoomDescription"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("RoomDescription")) : "",
                            RoomLocation = reader.GetString(reader.GetOrdinal("RoomLocation"))
                        });
                    }
                }
            }

            return rooms;
        }

        // Hent et enkelt lokale
        public Room GetRoomById(int roomID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT RoomID, RoomName, Size, RoomDescription, RoomLocation FROM Room WHERE RoomID = @RoomID";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@RoomID", roomID);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Room
                        {
                            RoomID = reader.GetInt32(reader.GetOrdinal("RoomID")),
                            RoomName = reader.GetString(reader.GetOrdinal("RoomName")),
                            Size = reader.GetString(reader.GetOrdinal("Size")),
                            RoomDescription = reader["RoomDescription"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("RoomDescription")) : "",
                            RoomLocation = reader.GetString(reader.GetOrdinal("RoomLocation"))
                        };
                    }
                }
            }

            return null;
        }

        // Opret nyt lokale
        public void AddRoom(Room room)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Room (RoomName, Size, RoomDescription, RoomLocation)
                               VALUES (@RoomName, @Size, @RoomDescription, @RoomLocation)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@RoomName", room.RoomName);
                cmd.Parameters.AddWithValue("@Size", room.Size);
                cmd.Parameters.AddWithValue("@RoomDescription", string.IsNullOrEmpty(room.RoomDescription) ? "" : room.RoomDescription);
                cmd.Parameters.AddWithValue("@RoomLocation", room.RoomLocation);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Opdater eksisterende lokale
        public void UpdateRoom(Room room)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Room 
                               SET RoomName = @RoomName,
                                   Size = @Size,
                                   RoomDescription = @RoomDescription,
                                   RoomLocation = @RoomLocation
                               WHERE RoomID = @RoomID";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@RoomID", room.RoomID);
                cmd.Parameters.AddWithValue("@RoomName", room.RoomName);
                cmd.Parameters.AddWithValue("@Size", room.Size);
                cmd.Parameters.AddWithValue("@RoomDescription", string.IsNullOrEmpty(room.RoomDescription) ? "" : room.RoomDescription);
                cmd.Parameters.AddWithValue("@RoomLocation", room.RoomLocation);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Slet lokale (inkl. alle bookinger tilknyttet)
        public void DeleteRoom(int roomID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Slet alle bookinger for dette lokale først
                string deleteBookingsSql = "DELETE FROM Booking WHERE RoomID = @RoomID";
                SqlCommand cmdBookings = new SqlCommand(deleteBookingsSql, conn);
                cmdBookings.Parameters.AddWithValue("@RoomID", roomID);
                cmdBookings.ExecuteNonQuery();

                // Slet selve lokalet
                string deleteRoomSql = "DELETE FROM Room WHERE RoomID = @RoomID";
                SqlCommand cmdRoom = new SqlCommand(deleteRoomSql, conn);
                cmdRoom.Parameters.AddWithValue("@RoomID", roomID);
                cmdRoom.ExecuteNonQuery();
            }
        }
    }
}
