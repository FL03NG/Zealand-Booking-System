using System;
using System.Collections.Generic;
using System.Text;
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
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Room (RoomName, Size, RoomDescription, RoomLocation, RoomType, HasSmartBoard) " +
                    "VALUES (@RoomName, @Size, @RoomDescription, @RoomLocation, @RoomType, @HasSmartBoard)", conn);

                cmd.Parameters.AddWithValue("@RoomName", room.RoomName);
                cmd.Parameters.AddWithValue("@Size", room.Size);
                cmd.Parameters.AddWithValue("@RoomDescription", room.RoomDescription ?? "");
                cmd.Parameters.AddWithValue("@RoomLocation", room.RoomLocation);
                cmd.Parameters.AddWithValue("@RoomType", (int)room.RoomType);      // enum -> int
                cmd.Parameters.AddWithValue("@HasSmartBoard", room.HasSmartBoard); // bool -> bit

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Room> GetAllRooms()
        {
            List<Room> rooms = new List<Room>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT RoomID, RoomName, Size, RoomDescription, RoomLocation, RoomType, HasSmartBoard FROM Room", conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Room r = new Room();
                        r.RoomID = (int)reader["RoomID"];
                        r.RoomName = (string)reader["RoomName"];
                        r.Size = (string)reader["Size"];
                        r.RoomDescription = reader["RoomDescription"].ToString();
                        r.RoomLocation = reader["RoomLocation"].ToString();

                        // NYT: map RoomType (int -> enum)
                        int roomTypeValue = Convert.ToInt32(reader["RoomType"]);
                        r.RoomType = (RoomType)roomTypeValue;

                        // NYT: map HasSmartBoard (bit -> bool)
                        r.HasSmartBoard = Convert.ToBoolean(reader["HasSmartBoard"]);

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
                      SET RoomName = @RoomName,
                          Size = @Size,
                          RoomDescription = @RoomDescription,
                          RoomLocation = @RoomLocation,
                          RoomType = @RoomType,
                          HasSmartBoard = @HasSmartBoard
                      WHERE RoomID = @RoomID", conn);

                cmd.Parameters.AddWithValue("@RoomID", room.RoomID);
                cmd.Parameters.AddWithValue("@RoomName", room.RoomName);
                cmd.Parameters.AddWithValue("@Size", room.Size);
                cmd.Parameters.AddWithValue("@RoomDescription", room.RoomDescription ?? "");
                cmd.Parameters.AddWithValue("@RoomLocation", room.RoomLocation);
                cmd.Parameters.AddWithValue("@RoomType", (int)room.RoomType);
                cmd.Parameters.AddWithValue("@HasSmartBoard", room.HasSmartBoard);

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

                string sql = "SELECT RoomID, RoomName, RoomLocation, Size, RoomDescription, RoomType, HasSmartBoard " +
                             "FROM Room WHERE RoomID = @RoomID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RoomID", roomID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Room room = new Room();
                            room.RoomID = (int)reader["RoomID"];
                            room.RoomName = reader["RoomName"].ToString();
                            room.RoomLocation = reader["RoomLocation"].ToString();
                            room.Size = (string)reader["Size"];
                            room.RoomDescription = reader["RoomDescription"].ToString();

                            int roomTypeValue = Convert.ToInt32(reader["RoomType"]);
                            room.RoomType = (RoomType)roomTypeValue;

                            room.HasSmartBoard = Convert.ToBoolean(reader["HasSmartBoard"]);

                            return room;
                        }
                    }
                }
            }

            return null; // hvis ingen fundet
        }
    }
}
