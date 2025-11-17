using System;
using System.Collections.Generic;
using System.Linq;
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

            using (SqlConnection connection = new SqlConnection(_connectionString)

        }

    }
}
