using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Zealand_Booking_System_Library.Models;

namespace Zealand_Booking_System_Library.Repository
{
    /// <summary>
    /// Repository responsible for reading and writing room data to the database.
    /// This class encapsulates all SQL access, ensuring the rest of the system
    /// remains independent of the database structure and SQL queries.
    /// </summary>
    public class RoomCollectionRepo : IRoomRepository
    {
        /// <summary>
        /// Connection string is injected so the repository stays flexible
        /// and can point to different databases without modifying the code.
        /// </summary>
        private readonly string _connectionString;
        public RoomCollectionRepo(string connectionString)
        {
            _connectionString = connectionString;
        }
        /// <summary>
        /// Retrieves all rooms from the database.
        /// Keeping SQL logic here isolates persistence concerns
        /// and ensures a single place for handling schema changes.
        /// </summary>
        public List<Room> GetAllRooms()
        {
            List<Room> rooms = new List<Room>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT RoomID, RoomName, RoomDescription, RoomLocation, RoomType, HasSmartBoard FROM Room";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Mapping is done inline to keep DB concerns local,
                            // rather than spreading parsing logic across layers.
                            Room room = new Room();
                            room.RoomID = Convert.ToInt32(reader["RoomID"]);
                            room.RoomName = reader["RoomName"].ToString();
                            room.RoomDescription = reader["RoomDescription"] != DBNull.Value
                                ? reader["RoomDescription"].ToString()
                                : string.Empty;
                            room.RoomLocation = reader["RoomLocation"].ToString();
                            if (reader["RoomType"] != DBNull.Value)
                            {
                                int roomTypeValue = Convert.ToInt32(reader["RoomType"]);
                                room.RoomType = (RoomType)roomTypeValue;
                            }
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
        /// <summary>
        /// Retrieves a single room by ID.
        /// Returning null when no result is found allows the service layer
        /// to handle missing rooms consistently.
        /// </summary>
        public Room GetRoomById(int roomID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT RoomID, RoomName, RoomLocation, RoomDescription, RoomType, HasSmartBoard " +
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
            return null;
        }
        /// <summary>
        /// Inserts a new room into the database.
        /// All write operations stay in this layer so the structure and SQL logic
        /// are not spread across the codebase.
        /// </summary>
        public void AddRoom(Room room)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql =
                    "INSERT INTO Room (RoomName, RoomDescription, RoomLocation, RoomType, HasSmartBoard) " +
                    "VALUES (@RoomName, @RoomDescription, @RoomLocation, @RoomType, @HasSmartBoard)";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RoomName", room.RoomName);
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
        /// <summary>
        /// Updates an existing room.
        /// Centralizing update logic here ensures schema changes
        /// only require modifications in one place.
        /// </summary>
        public void UpdateRoom(Room room)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql =
                    @"UPDATE Room 
                      SET RoomName = @RoomName,
                          RoomDescription = @RoomDescription,
                          RoomLocation = @RoomLocation,
                          RoomType = @RoomType,
                          HasSmartBoard = @HasSmartBoard
                      WHERE RoomID = @RoomID";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RoomID", room.RoomID);
                    cmd.Parameters.AddWithValue("@RoomName", room.RoomName);
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
        /// <summary>
        /// Deletes a room and all related bookings.
        /// The repository handles cascading deletes manually
        /// to protect database integrity even if foreign keys are missing.
        /// </summary>
        public void DeleteRoom(int roomID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // Deleting bookings first avoids foreign key conflicts
                // and ensures no orphaned reservations remain.
                string deleteBookingsSql = "DELETE FROM Booking WHERE RoomID = @RoomID";
                using (SqlCommand cmdBookings = new SqlCommand(deleteBookingsSql, conn))
                {
                    cmdBookings.Parameters.AddWithValue("@RoomID", roomID);
                    cmdBookings.ExecuteNonQuery();
                }
                // Removing the room itself keeps the database consistent.
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
