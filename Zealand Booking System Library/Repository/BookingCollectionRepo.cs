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
            List<Booking> bookings = new List<Booking>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT 
                b.BookingID,
                b.BookingDescription,
                b.BookingDate,
                b.TimeSlot,
                b.AccountID,
                b.RoomID,
                a.Username,
                r.RoomName,
                r.Size,
                r.RoomDescription,
                r.RoomLocation
            FROM Booking b
            INNER JOIN Account a ON b.AccountID = a.AccountID
            INNER JOIN Room r ON b.RoomID = r.RoomID";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Booking booking = new Booking();
                    booking.BookingID = (int)reader["BookingID"];
                    booking.BookingDescription = reader["BookingDescription"] != DBNull.Value
                        ? reader["BookingDescription"].ToString()
                        : "";
                    booking.BookingDate = (DateTime)reader["BookingDate"];

                    int timeSlotValue = (int)reader["TimeSlot"];
                    booking.TimeSlot = (TimeSlot)timeSlotValue;

                    booking.AccountID = (int)reader["AccountID"];
                    booking.RoomID = (int)reader["RoomID"];

                    Account account = new Account();
                    account.AccountID = booking.AccountID;
                    account.Username = reader["Username"].ToString();
                    booking.Account = account;

                    Room room = new Room();
                    room.RoomID = booking.RoomID;
                    room.RoomName = reader["RoomName"].ToString();
                    room.Size = reader["Size"].ToString();
                    room.RoomDescription = reader["RoomDescription"].ToString();
                    room.RoomLocation = reader["RoomLocation"].ToString();
                    booking.Room = room;

                    bookings.Add(booking); // vigtigt!
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
                    "INSERT INTO Booking (BookingDescription, RoomID, AccountID, BookingDate, TimeSlot) " +
                    "VALUES (@BookingDescription, @RoomID, @AccountID, @BookingDate, @TimeSlot)",
                    connection);

                command.Parameters.AddWithValue("@BookingDescription",
                    string.IsNullOrEmpty(booking.BookingDescription) ? "" : booking.BookingDescription);
                command.Parameters.AddWithValue("@RoomID", booking.RoomID);
                command.Parameters.AddWithValue("@AccountID", booking.AccountID);
                command.Parameters.AddWithValue("@BookingDate", booking.BookingDate);
                command.Parameters.AddWithValue("@TimeSlot", (int)booking.TimeSlot);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public void Update(Booking booking)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "UPDATE Booking SET BookingDate = @BookingDate, AccountID = @AccountID, TimeSlot = @TimeSlot WHERE BookingID = @BookingID",
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
                SqlCommand command = new SqlCommand("DELETE FROM Booking WHERE BookingID = @BookingID", connection);
                command.Parameters.AddWithValue("@BookingID", id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

    }
}
