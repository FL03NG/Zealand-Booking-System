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
            using (SqlConnection conn = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=VaskEnTidDataBase;Trusted_Connection=True;TrustServerCertificate=True;"))
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

            using (SqlConnection conn = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=VaskEnTidDataBase;Trusted_Connection=True;TrustServerCertificate=True;"))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Room", conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Resident r = new Resident
                        {
                            ResidentID = (int)reader["ResidentID"],
                            PhoneNumber = (string)reader["PhoneNumber"],
                            ResidentName = (string)reader["ResidentName"],
                            City = (string)reader["City"],
                            Email = (string)reader["Email"],
                            PostNr = (string)reader["PostNr"],
                            ApartmentNr = (string)reader["ApartmentNr"],
                            FloorNr = (int)reader["FloorNr"]
                        };
                        rooms.Add(r);
                    }
                }
            }
            return rooms;
        }

        public void UpdateRoom(Room room)
        {
            using (SqlConnection conn = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=VaskEnTidDataBase;Trusted_Connection=True;TrustServerCertificate=True;"))
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Resident SET ResidentID=@ResidentId, PhoneNumber=@PhoneNumber, ResidentName = @ResidentName, City=@City, PostNr=@PostNr, Email=@Email, " +
                    "ApartmentNr=@ApartmentNr, ApartmentNr=@ApartmentNr, FloorNr=@FloorNr", conn);

                cmd.Parameters.AddWithValue("@ResidentID", resident.ResidentID);
                cmd.Parameters.AddWithValue("@PhoneNumber", resident.PhoneNumber);
                cmd.Parameters.AddWithValue("@ResidentName", resident.ResidentName);
                cmd.Parameters.AddWithValue("@City", resident.City);
                cmd.Parameters.AddWithValue("@PostNr", resident.PostNr);
                cmd.Parameters.AddWithValue("@Email", resident.Email);
                cmd.Parameters.AddWithValue("@ApartmentNr", resident.ApartmentNr);
                cmd.Parameters.AddWithValue("@FloorNr", resident.FloorNr);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void DeleteRoom(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Resident WHERE ResidentID=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
