using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System.Pages.Shared
{
    public class BookingListModel : PageModel
    {
        //forbindelse til database
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

        private BookingService _bookingService;

        public string Message { get; private set; }
        public void OnGet()
        {
            LoadData();
        }
        public void OnPost()
        {
            BookingCollectionRepo bookingRepo = new BookingCollectionRepo(_connectionString);
            BookingService bookingService = new BookingService(bookingRepo);

            try
            {
                bookingService.Add(NewBooking);
                Message = "Booking oprettet!";
            }
            catch (System.Exception ex)
            {
                Message = "Fejl: " + ex.Message;
            }

            LoadData();
        }
        // Når der slettes en booking (POST Delete)
        public IActionResult OnPostDelete(int bookingID)
        {
            BookingCollectionRepo bookingRepo = new BookingCollectionRepo(_connectionString);
            BookingService bookingService = new BookingService(bookingRepo);

            try
            {
                bookingService.Delete(bookingID);
                Message = "Booking slettet!";
            }
            catch (System.Exception ex)
            {
                Message = "Fejl ved sletning: " + ex.Message;
            }

            LoadData();
            return Page(); // behold besked og opdateret liste
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