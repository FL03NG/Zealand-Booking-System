using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System.Pages
{
    public class FindRoomsModel : PageModel
    {
        private readonly string _connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=RoomBooking;Trusted_Connection=True;TrustServerCertificate=True";

        private readonly BookingService _bookingService;

        public FindRoomsModel()
        {
            BookingCollectionRepo bookingRepo = new BookingCollectionRepo(_connectionString);
            RoomCollectionRepo roomRepo = new RoomCollectionRepo(_connectionString);

            _bookingService = new BookingService(bookingRepo, roomRepo);
        }

        // --- FILTER ---
        [BindProperty]
        public DateTime SelectedDate { get; set; }

        [BindProperty]
        public TimeSlot SelectedTimeSlot { get; set; }

        [BindProperty]
        public RoomType? SelectedRoomType { get; set; }

        // --- BOOKING PROPS ---
        [BindProperty]
        public int RoomID { get; set; }

        // --- ROOM AVAILABILITY ---
        public List<RoomAvailability> Rooms { get; private set; } = new();

        // --- GET ---
        public void OnGet()
        {
            SelectedDate = DateTime.Today;
            SelectedTimeSlot = TimeSlot.Slot08_10;
            SelectedRoomType = null;

            LoadRooms();
        }

        // --- POST: filtrer liste ---
        public void OnPost()
        {
            if (SelectedDate == DateTime.MinValue)
            {
                SelectedDate = DateTime.Today;
            }

            LoadRooms();
        }

        // --- POST: book et lokale ---
        public IActionResult OnPostBook()
        {
            int? accountId = HttpContext.Session.GetInt32("AccountID");
            if (accountId == null)
            {
                TempData["Message"] = "Du skal være logget ind for at booke et lokale.";
                return RedirectToPage("/BookingList");
            }

            Booking newBooking = new Booking
            {
                RoomID = RoomID,
                BookingDate = SelectedDate,
                TimeSlot = SelectedTimeSlot,
                AccountID = accountId.Value
            };

            try
            {
                _bookingService.Add(newBooking);
                TempData["Message"] = "Booking oprettet!";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Fejl: " + ex.Message;
            }

            return RedirectToPage(new
            {
                SelectedDate,
                SelectedTimeSlot,
                SelectedRoomType
            });
        }

        // --- HJÆLPEMETODE ---
        private void LoadRooms()
        {
            Rooms = _bookingService.GetRoomAvailability(SelectedDate, SelectedTimeSlot, SelectedRoomType);
        }
    }
}