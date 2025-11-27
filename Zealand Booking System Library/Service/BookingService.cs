using System;
using System.Collections.Generic;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;

namespace Zealand_Booking_System_Library.Service
{
    public class BookingService
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IRoomRepository _roomRepo;

        public BookingService(IBookingRepository bookingRepo, IRoomRepository roomRepo)
        {
            _bookingRepo = bookingRepo;
            _roomRepo = roomRepo;
        }

        public void Add(Booking booking)
        {
            // 1) Find rummet for at se RoomType
            Room room = _roomRepo.GetRoomById(booking.RoomID);
            if (room == null)
            {
                throw new Exception("Lokalet findes ikke.");
            }

            int maxBookingsForRoom;

            if (room.RoomType == RoomType.ClassRoom)
            {
                maxBookingsForRoom = 2;  // ClassRoom: må deles af 2
            }
            else if (room.RoomType == RoomType.MeetingRoom)
            {
                maxBookingsForRoom = 1;  // MeetingRoom: kun 1 booking ad gangen
            }
            else
            {
                maxBookingsForRoom = 1;  // fallback
            }

            List<Booking> allbookings = _bookingRepo.GetAll();
            int sameRoomSameSlotCount = 0;

            // NYT: tæller hvor mange bookinger denne bruger allerede har
            int userBookingCount = 0;

            foreach (Booking existing in allbookings)
            {
                bool sameDay = existing.BookingDate.Date == booking.BookingDate.Date;
                bool sameSlot = existing.TimeSlot == booking.TimeSlot;
                bool sameUser = existing.AccountID == booking.AccountID;
                bool sameRoom = existing.RoomID == booking.RoomID;

                // tæller alle bookinger for brugeren (uanset dato)
                if (sameUser)
                {
                    userBookingCount++;
                }

                // 2) Samme bruger må ikke have to bookinger i samme tidsrum
                if (sameUser && sameDay && sameSlot)
                {
                    throw new Exception("Du har allerede en booking i dette tidsrum.");
                }

                // 3) Tæl bookinger på samme lokale, dato og tidsrum
                if (sameRoom && sameDay && sameSlot)
                {
                    sameRoomSameSlotCount++;
                }
            }

            // NYT: max 5 bookinger per bruger
            if (userBookingCount >= 5)
            {
                throw new Exception("Du har allerede 5 bookinger. Slet en booking, før du opretter en ny.");
            }

            // 4) Tjek om vi rammer max for lokale-typen
            if (sameRoomSameSlotCount >= maxBookingsForRoom)
            {
                if (room.RoomType == RoomType.ClassRoom)
                {
                    throw new Exception("Dette klasselokale er allerede booket af to brugere i dette tidsrum.");
                }
                else
                {
                    throw new Exception("Dette mødelokale er allerede booket i dette tidsrum.");
                }
            }

            // 5) Alt ok → gem booking
            _bookingRepo.Add(booking);
        }

        public Booking GetBookingById(int bookingID)
        {
            return _bookingRepo.GetBookingById(bookingID);
        }

        public void Delete(int id)
        {
            _bookingRepo.Delete(id);
        }

        public List<Booking> GetAll()
        {
            return _bookingRepo.GetAll();
        }

        public void Update(Booking booking)
        {
            _bookingRepo.Update(booking);
        }
    }
}
