using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        [BindProperty]
        public DateTime SelectedDate { get; set; }

        [BindProperty]
        public TimeSlot SelectedTimeSlot { get; set; }

        [BindProperty]
        public RoomType? SelectedRoomType { get; set; }

        // VIGTIGT: RoomAvailability, ikke Room
        public List<RoomAvailability> Rooms { get; private set; }

        public void OnGet()
        {
            SelectedDate = DateTime.Today;
            SelectedTimeSlot = TimeSlot.Slot08_10;   // standard
            SelectedRoomType = null;                 // "alle typer"

            LoadRooms();
        }

        public void OnPost()
        {
            if (SelectedDate == DateTime.MinValue)
            {
                SelectedDate = DateTime.Today;
            }

            LoadRooms();
        }

        private void LoadRooms()
        {
            Rooms = _bookingService.GetRoomAvailability(SelectedDate, SelectedTimeSlot, SelectedRoomType);
        }
    }
}
