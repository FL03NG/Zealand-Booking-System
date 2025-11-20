using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System.Pages.Shared
{
    public class BookingListModel : PageModel
    {
        private readonly string _connectionString =
            "Server =(localdb)\\MSSQLLocalDB;Database=RoomBooking;Trusted_Connection=True;TrustServerCertificate=True";

        // Lister til visning
        public List<Booking> Bookings { get; private set; }
        public List<Room> Rooms { get; private set; }
        public List<Account> Users { get; private set; }

        // Ny booking (formular i bunden – hvis du bruger den)
        [BindProperty]
        public Booking NewBooking { get; set; }

        // 🔎 søgning
        [BindProperty]
        public string SearchName { get; set; }

        // ✏ redigering
        [BindProperty]
        public int EditBookingID { get; set; }

        [BindProperty]
        public Booking EditBooking { get; set; }

        private readonly BookingService _bookingService;

        public string Message { get; private set; }

        public BookingListModel()
        {
            BookingCollectionRepo bookingRepo = new BookingCollectionRepo(_connectionString);
            _bookingService = new BookingService(bookingRepo);
        }

        public void OnGet()
        {
            LoadData();
        }

        // Opret booking (hvis du bruger formularen nederst)
        public void OnPost()
        {
            try
            {
                _bookingService.Add(NewBooking);
                Message = "Booking oprettet!";
            }
            catch (Exception ex)
            {
                Message = "Fejl: " + ex.Message;
            }

            LoadData();
        }

        // 🔎 Søg på brugernavn
        public void OnPostSearch()
        {
            LoadData(); // henter Bookings + Users

            if (string.IsNullOrWhiteSpace(SearchName))
            {
                return; // tom søgning → behold alle
            }

            string searchLower = SearchName.ToLower();
            List<Booking> filtered = new List<Booking>();

            if (Bookings == null)
            {
                return;
            }

            for (int i = 0; i < Bookings.Count; i++)
            {
                Booking booking = Bookings[i];
                string username = null;

                if (booking.Account != null)
                {
                    username = booking.Account.Username;
                }
                else if (Users != null)
                {
                    for (int j = 0; j < Users.Count; j++)
                    {
                        if (Users[j].AccountID == booking.AccountID)
                        {
                            username = Users[j].Username;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(username))
                {
                    if (username.ToLower().Contains(searchLower))
                    {
                        filtered.Add(booking);
                    }
                }
            }

            Bookings = filtered;
        }

        // 🗑 Slet booking
        public IActionResult OnPostDelete(int bookingID)
        {
            try
            {
                _bookingService.Delete(bookingID);
                Message = "Booking slettet!";
            }
            catch (Exception ex)
            {
                Message = "Fejl ved sletning: " + ex.Message;
            }

            LoadData();
            return Page();
        }

        // ✏ Start redigering
        public IActionResult OnPostStartEdit(int bookingID)
        {
            Message = "StartEdit ramte: " + bookingID;
            EditBookingID = bookingID;
            EditBooking = _bookingService.GetBookingById(bookingID);
            LoadData();
            return Page();
        }

        // 💾 Gem redigering
        public IActionResult OnPostSaveEdit()
        {
            try
            {
                _bookingService.Update(EditBooking);
                Message = "Booking er ændret!";
            }
            catch (Exception ex)
            {
                Message = "Fejl under redigering: " + ex.Message;
                LoadData();
                return Page();
            }

            // Hent de opdaterede data og bliv på siden
            LoadData();
            return Page();
        }

        // Fælles load
        private void LoadData()
        {
            Bookings = _bookingService.GetAll();

            RoomCollectionRepo roomRepo = new RoomCollectionRepo(_connectionString);
            Rooms = roomRepo.GetAllRooms();

            UserCollectionRepo userRepo = new UserCollectionRepo(_connectionString);
            Users = userRepo.GetAllUsers();
        }
    }
}
