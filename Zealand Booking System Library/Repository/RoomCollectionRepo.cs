using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Zealand_Booking_System_Library.Models;

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
            List<Room> rooms = new List<Room>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT RoomID, RoomName, Size, RoomDescription, RoomLocation, RoomType, HasSmartBoard FROM Room";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Room room = new Room();

                            room.RoomID = Convert.ToInt32(reader["RoomID"]);
                            room.RoomName = reader["RoomName"].ToString();
                            room.Size = reader["Size"].ToString();
                            room.RoomDescription = reader["RoomDescription"] != DBNull.Value
                                ? reader["RoomDescription"].ToString()
                                : string.Empty;
                            room.RoomLocation = reader["RoomLocation"].ToString();

                            // RoomType (int -> enum), tjek for NULL
                            if (reader["RoomType"] != DBNull.Value)
                            {
                                int roomTypeValue = Convert.ToInt32(reader["RoomType"]);
                                room.RoomType = (RoomType)roomTypeValue;
                            }

                            // HasSmartBoard (bit -> bool), tjek for NULL
                            if (reader["HasSmartBoard"] != DBNull.Value)
                            {
                                room.HasSmartBoard = Convert.ToBoolean(reader["HasSmartBoard"]);
                            }

                            rooms.Add(room);
                        }
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
                string sql = "SELECT RoomID, RoomName, RoomLocation, Size, RoomDescription, RoomType, HasSmartBoard " +
                             "FROM Room WHERE RoomID = @RoomID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RoomID", roomID);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Room room = new Room();

                            room.RoomID = Convert.ToInt32(reader["RoomID"]);
                            room.RoomName = reader["RoomName"].ToString();
                            room.RoomLocation = reader["RoomLocation"].ToString();
                            room.Size = reader["Size"].ToString();
                            room.RoomDescription = reader["RoomDescription"] != DBNull.Value
                                ? reader["RoomDescription"].ToString()
                                : string.Empty;

                            if (reader["RoomType"] != DBNull.Value)
                            {
                                int roomTypeValue = Convert.ToInt32(reader["RoomType"]);
                                room.RoomType = (RoomType)roomTypeValue;
                            }

                            if (reader["HasSmartBoard"] != DBNull.Value)
                            {
                                room.HasSmartBoard = Convert.ToBoolean(reader["HasSmartBoard"]);
                            }

                            return room;
                        }
                    }
                }
            }

            return null; // ingen fundet
        }

        // Opret nyt lokale
        public void AddRoom(Room room)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql =
                    "INSERT INTO Room (RoomName, Size, RoomDescription, RoomLocation, RoomType, HasSmartBoard) " +
                    "VALUES (@RoomName, @Size, @RoomDescription, @RoomLocation, @RoomType, @HasSmartBoard)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RoomName", room.RoomName);
                    cmd.Parameters.AddWithValue("@Size", room.Size);
                    cmd.Parameters.AddWithValue("@RoomDescription",
                        string.IsNullOrEmpty(room.RoomDescription) ? string.Empty : room.RoomDescription);
                    cmd.Parameters.AddWithValue("@RoomLocation", room.RoomLocation);
                    cmd.Parameters.AddWithValue("@RoomType", (int)room.RoomType);
                    cmd.Parameters.AddWithValue("@HasSmartBoard", room.HasSmartBoard);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Opdater eksisterende lokale
        public void UpdateRoom(Room room)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql =
                    @"UPDATE Room 
                      SET RoomName = @RoomName,
                          Size = @Size,
                          RoomDescription = @RoomDescription,
                          RoomLocation = @RoomLocation,
                          RoomType = @RoomType,
                          HasSmartBoard = @HasSmartBoard
                      WHERE RoomID = @RoomID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RoomID", room.RoomID);
                    cmd.Parameters.AddWithValue("@RoomName", room.RoomName);
                    cmd.Parameters.AddWithValue("@Size", room.Size);
                    cmd.Parameters.AddWithValue("@RoomDescription",
                        string.IsNullOrEmpty(room.RoomDescription) ? string.Empty : room.RoomDescription);
                    cmd.Parameters.AddWithValue("@RoomLocation", room.RoomLocation);
                    cmd.Parameters.AddWithValue("@RoomType", (int)room.RoomType);
                    cmd.Parameters.AddWithValue("@HasSmartBoard", room.HasSmartBoard);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Slet lokale (inkl. bookinger)
        public void DeleteRoom(int roomID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Først slet alle bookinger tilknyttet lokalet
                string deleteBookingsSql = "DELETE FROM Booking WHERE RoomID = @RoomID";
                using (SqlCommand cmdBookings = new SqlCommand(deleteBookingsSql, conn))
                {
                    cmdBookings.Parameters.AddWithValue("@RoomID", roomID);
                    cmdBookings.ExecuteNonQuery();
                }

                // Derefter selve lokalet
                string deleteRoomSql = "DELETE FROM Room WHERE RoomID = @RoomID";
                using (SqlCommand cmdRoom = new SqlCommand(deleteRoomSql, conn))
                {
                    cmdRoom.Parameters.AddWithValue("@RoomID", roomID);
                    cmdRoom.ExecuteNonQuery();
                }
            }
        }
    }
}
