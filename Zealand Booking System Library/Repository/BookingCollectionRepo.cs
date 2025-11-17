using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;

namespace Zealand_Booking_System_Library.Repository
{
    public class BookingCollectionRepo : IBookingRepository
    {
        private readonly string _connectionString;
        public BookingCollectionRepo(string connectionString)    
        {
            _connectionString = connectionString;
        }
        public List<Booking> GetAll()
        {
            List<Booking > bookings = new List<Booking>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT 
                        b.BookingID, 
                        b.BookingDate, 
                        b.AccountID,  
                        b.TimeSlot,
                        r.ResidentName, 
                    FROM Bookings b
                    LEFT JOIN Resident r ON b.ResidentID = r.ResidentID
                    LEFT JOIN Machine m ON b.MachineID = m.MachineID";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Booking b = new Booking();
                    b.BookingID = (int)reader["BookingID"];
                    b.BookingDate = (DateTime)reader["BookingDate"];
                    b.AccountID = (int)reader["AccountID"];
                    b.TimeSlot = Enum.Parse<TimeSlot>(reader["TimeSlot"].ToString());

                    // Fyld beboer
                    Account res = new Account();
                    res.AccountID = b.AccountID;
                    res.Username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : "";

                    b.Account = res;

                    // Fyld maskine
                    string Rooms = reader["Room"] != DBNull.Value ? reader["Room"].ToString() : "";

                }
            }
            return bookings;
        }
        // Opret booking
        public void Add(Booking booking)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "INSERT INTO Bookings (BookingDate, AccountID, TimeSlot) VALUES (@BookingDate, @AccountID, @TimeSlot)",
                    connection);

                command.Parameters.AddWithValue("@BookingDate", booking.BookingDate);
                command.Parameters.AddWithValue("@AccountID", booking.AccountID);
                command.Parameters.AddWithValue("@TimeSlot", booking.TimeSlot.ToString());

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public void Update(Booking booking)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "UPDATE Bookings SET BookingDate = @BookingDate, AccountID = @AccountID, TimeSlot = @TimeSlot WHERE BookingID = @BookingID",
                    connection);

                command.Parameters.AddWithValue("@BookingID", booking.BookingID);
                command.Parameters.AddWithValue("@BookingDate", booking.BookingDate);
                command.Parameters.AddWithValue("@AccountID", booking.AccountID);
                command.Parameters.AddWithValue("@TimeSlot", booking.TimeSlot.ToString());

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        // Slet booking
        public void Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("DELETE FROM Bookings WHERE BookingID = @BookingID", connection);
                command.Parameters.AddWithValue("@BookingID", id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

    }
}
