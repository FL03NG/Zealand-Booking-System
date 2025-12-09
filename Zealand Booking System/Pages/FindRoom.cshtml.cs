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
    /// <summary>
    /// PageModel responsible for searching available rooms and creating bookings.
    /// It manages user-selected filters and delegates availability calculations
    /// and booking rules to the service layer.
    /// </summary>
    public class FindRoomsModel : PageModel
    {
        /// <summary>
        /// Connection string used to configure repository access.
        /// Persistence details remain encapsulated within repositories.
        /// </summary>
        private readonly string _connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=RoomBooking;Trusted_Connection=True;TrustServerCertificate=True";
        /// <summary>
        /// Service responsible for booking logic and room availability rules.
        /// </summary>
        private readonly BookingService _bookingService;

        /// <summary>
        /// Message shown to the user (e.g. after updating the list or creating a booking).
        /// </summary>
        public string Message { get; private set; }
        /// <summary>
        /// Initializes required repositories and composes the booking service.
        /// </summary>
        public FindRoomsModel()
        {
            BookingCollectionRepo bookingRepo = new BookingCollectionRepo(_connectionString);
            RoomCollectionRepo roomRepo = new RoomCollectionRepo(_connectionString);

            _bookingService = new BookingService(bookingRepo, roomRepo);
        }
        /// <summary>
        /// Selected date used to search for available rooms.
        /// </summary>
        [BindProperty]
        public DateTime SelectedDate { get; set; }
        /// <summary>
        /// Selected time slot used to determine room availability.
        /// </summary>
        [BindProperty]
        public TimeSlot SelectedTimeSlot { get; set; }
        /// <summary>
        /// Optional room type filter.
        /// Allows narrowing results without enforcing a mandatory selection.
        /// </summary>
        [BindProperty]
        public RoomType? SelectedRoomType { get; set; }
        /// <summary>
        /// Identifies the room the user wants to book.
        /// Bound from the booking form submission.
        /// </summary>
        [BindProperty]
        public int RoomID { get; set; }
        /// <summary>
        /// Collection of rooms with calculated availability information.
        /// This is populated by the service layer to keep logic out of the UI.
        /// </summary>
        public List<RoomAvailability> Rooms { get; private set; } = new();
        /// <summary>
        /// Initializes default filter values and loads room availability
        /// for the first page visit.
        /// </summary>
        public void OnGet()
        {
            SelectedDate = DateTime.Today;
            SelectedTimeSlot = TimeSlot.Slot08_10;
            SelectedRoomType = null;
            LoadRooms();
        }
        /// <summary>
        /// Refreshes the room list based on user-selected filters.
        /// Default values are restored if required data is missing.
        /// </summary>
        public void OnPost()
        {
            if (SelectedDate == DateTime.MinValue)
            {
                SelectedDate = DateTime.Today;
            }
            LoadRooms();
            Message = "Listen er opdateret.";
        }
        /// <summary>
        /// Creates a booking for the selected room.
        /// Ensures the user is authenticated before allowing a booking.
        /// </summary>
        public IActionResult OnPostBook()
        {
            int? accountId = HttpContext.Session.GetInt32("AccountID");
            if (accountId == null)
            {
                TempData["Message"] = "You need to be logged in to make a booking.";
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
                TempData["Message"] = "Booking made!";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error: " + ex.Message;
            }
            return RedirectToPage(new
            {
                SelectedDate,
                SelectedTimeSlot,
                SelectedRoomType
            });
        }
        /// <summary>
        /// Loads room availability based on the selected filters.
        /// Centralizing this logic avoids duplication across handlers.
        /// </summary>
        private void LoadRooms()
        {
            Rooms = _bookingService.GetRoomAvailability(SelectedDate, SelectedTimeSlot, SelectedRoomType);
        }
    }
}