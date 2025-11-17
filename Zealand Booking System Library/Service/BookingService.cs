using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Models;
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
        public void Add(Booking booking)
        {
            List<Booking> allbookings = _bookingRepo.GetAll();

            foreach (Booking existing in allbookings) 
            {
                bool sameUser = existing.AccountID == booking.AccountID;
                bool futureOrSameDay = existing.BookingDate == booking.BookingDate;

                if (sameUser && futureOrSameDay) 
                {
                    throw new Exception("Denne user har allerede en aktiv booking");
                }
            }
            foreach (Booking existing in allbookings)
            {
                bool sameRoom = existing.RoomID == booking.RoomID;
                bool sameDate = existing.BookingDate.Date == booking.BookingDate.Date;
                bool sameSlot = existing.TimeSlot == booking.TimeSlot;

                if (sameRoom && sameDate && sameSlot)
                {
                    throw new Exception("Dette rum er allrede booket i dette tidsrum");
                }
            }

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
