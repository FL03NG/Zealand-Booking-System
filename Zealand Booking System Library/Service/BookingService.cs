using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;

namespace Zealand_Booking_System_Library.Service
{
    public class BookingService
    {
        private readonly IBookingRepository _bookingRepo;
        public BookingService(IBookingRepository bookingRepo)
        {
            _bookingRepo = bookingRepo;
        }
        public void Add(Booking booking)
        {
            List<Booking> allbookings = _bookingRepo.GetAll();

            // Tjek: samme bruger samme dato
            foreach (Booking existing in allbookings)
            {
                bool sameUser = existing.AccountID == booking.AccountID;
                bool sameDay = existing.BookingDate.Date == booking.BookingDate.Date;

                if (sameUser && sameDay)
                {
                    throw new Exception("Denne user har allerede en aktiv booking på denne dato");
                }
            }

            // Tjek: samme room + dato + tidsrum
            foreach (Booking existing in allbookings)
            {
                bool sameRoom = existing.RoomID == booking.RoomID;
                bool sameDate = existing.BookingDate.Date == booking.BookingDate.Date;
                bool sameSlot = existing.TimeSlot == booking.TimeSlot;

                if (sameRoom && sameDate && sameSlot)
                {
                    throw new Exception("Dette rum er allerede booket i dette tidsrum");
                }
            }
            _bookingRepo.Add(booking);
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
