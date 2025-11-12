using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Repository;

namespace Zealand_Booking_System_Library.Service
{
    internal class BookingService
    {
        private readonly IBookingRepository _bookingRepo;
        public BookingService(IBookingRepository bookingRepo)
        {
            _bookingRepo = bookingRepo;
        }
        public void Add()
        {

        }
        public void Delete(int id)
        {
            _bookingRepo.Delete(id);
        }
        public List<Models.Booking> GetAll()
        {
            return _bookingRepo.GetAll();
        }
        public void Update(Models.Booking booking)
        {
            _bookingRepo.Update(booking);
        }
    }
}
