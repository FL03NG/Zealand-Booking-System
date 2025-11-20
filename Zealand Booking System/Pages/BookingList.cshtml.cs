using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Components.Forms;
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
        // forbindelse til database
        private readonly string _connectionString =
            "Server =(localdb)\\MSSQLLocalDB;Database=RoomBooking;Trusted_Connection=True;TrustServerCertificate=True";

        // Lister som bruges på siden
        public List<Booking> Bookings { get; private set; }
        public List<Room> Rooms { get; private set; }
        public List<Account> Users { get; private set; }
        [BindProperty]
        public int EditBookingID { get; set; }

        [BindProperty]
        public Booking EditBooking { get; set; }
        [BindProperty]
        public Booking NewBooking { get; set; }

        // 🔎 søgetekst
        [BindProperty]
        public string SearchName { get; set; }

        private BookingService _bookingService;

        public string Message { get; private set; }

        public void OnGet()
        {
            LoadData();
        }

        // standard POST → opret booking
        public void OnPost()
        {
            BookingCollectionRepo bookingRepo = new BookingCollectionRepo(_connectionString);
            BookingService bookingService = new BookingService(bookingRepo);

            try
            {
                bookingService.Add(NewBooking);
                Message = "Booking oprettet!";
            }
            catch (Exception ex)
            {
                Message = "Fejl: " + ex.Message;
            }

            LoadData();
        }

        // 🔎 POST: søg på brugernavn
        public void OnPostSearch()
        {
            LoadData(); // henter Bookings + Users

            Message = "Søgning udført."; // bare så du kan se at handleren bliver ramt

            if (string.IsNullOrWhiteSpace(SearchName))
            {
                // tom søgning → vis alle
                return;
            }

            string searchLower = SearchName.ToLower();
            List<Booking> filtered = new List<Booking>();

            if (Bookings == null)
            {
                return;
            }

            int i;
            for (i = 0; i < Bookings.Count; i++)
            {
                Booking booking = Bookings[i];

                string username = null;

                // prøv først via navigation property
                if (booking.Account != null)
                {
                    username = booking.Account.Username;
                }
                else if (Users != null)
                {
                    int j;
                    for (j = 0; j < Users.Count; j++)
                    {
                        if (Users[j].AccountID == booking.AccountID)
                        {
                            username = Users[j].Username;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(username))
                {
                    string usernameLower = username.ToLower();
                    if (usernameLower.Contains(searchLower))
                    {
                        filtered.Add(booking);
                    }
                }
            }

            Bookings = filtered;
        }

        // slet booking
        public IActionResult OnPostDelete(int bookingID)
        {
            BookingCollectionRepo bookingRepo = new BookingCollectionRepo(_connectionString);
            BookingService bookingService = new BookingService(bookingRepo);

            try
            {
                bookingService.Delete(bookingID);
                Message = "Booking slettet!";
            }
            catch (Exception ex)
            {
                Message = "Fejl ved sletning: " + ex.Message;
            }

            LoadData();
            return Page();
        }

        // Hjælpefunktion til at hente data
        private void LoadData()
        {
            BookingCollectionRepo bookingRepo = new BookingCollectionRepo(_connectionString);
            BookingService bookingService = new BookingService(bookingRepo);
            Bookings = bookingService.GetAll();

            RoomCollectionRepo roomRepo = new RoomCollectionRepo(_connectionString);
            Rooms = roomRepo.GetAllRooms();

            UserCollectionRepo userRepo = new UserCollectionRepo(_connectionString);
            Users = userRepo.GetAllUsers();
        }
        public IActionResult OnPostStartEdit(int bookingID)
        {
            EditBookingID = bookingID;
            EditBooking = _bookingService.GetBookingById(bookingID);
            Bookings = _bookingService.GetAll(); // reload list
            return Page();
        }
        public IActionResult OnPostSaveEdit()
        {
            try
            {
                _bookingService.Update(EditBooking);
                TempData["Message"] = "Booking er ændret!";
            }
            catch (System.Exception ex)
            {
                TempData["Message"] = "Fejl under redigering: " + ex.Message;
            }

            return RedirectToPage(); // reload

        }
    }
}