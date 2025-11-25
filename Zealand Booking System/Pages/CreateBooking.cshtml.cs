using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System.Pages
{
    public class BookingModel : PageModel
    {
        private readonly string _connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=RoomBooking;Trusted_Connection=True;TrustServerCertificate=True";

        public List<Booking> Bookings { get; private set; }
        public List<Room> Rooms { get; private set; }

        [BindProperty]
        public Booking NewBooking { get; set; }

        public string Message { get; private set; }

        public void OnGet()
        {
            LoadData();
        }

        public IActionResult OnPost()
        {
            // Hent logget bruger fra session
            int? accountId = HttpContext.Session.GetInt32("AccountID");
            if (accountId == null)
            {
                Message = "Du skal være logget ind for at oprette booking.";
                LoadData();
                return Page();
            }

            // Sæt AccountID på booking
            NewBooking.AccountID = accountId.Value;

            BookingCollectionRepo bookingRepo = new BookingCollectionRepo(_connectionString);
            BookingService bookingService = new BookingService(bookingRepo);

            try
            {
                bookingService.Add(NewBooking);
                Message = "Booking oprettet!";
                return RedirectToPage(); // genindlæs siden med ny booking
            }
            catch (Exception ex)
            {
                Message = "Fejl: " + ex.Message;
                LoadData();
                return Page();
            }
        }

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

        private void LoadData()
        {
            BookingCollectionRepo bookingRepo = new BookingCollectionRepo(_connectionString);
            BookingService bookingService = new BookingService(bookingRepo);
            Bookings = bookingService.GetAll();

            RoomCollectionRepo roomRepo = new RoomCollectionRepo(_connectionString);
            Rooms = roomRepo.GetAllRooms();
        }
    }
}