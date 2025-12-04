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

        // List data
        public List<Booking> Bookings { get; private set; }
        public List<Room> Rooms { get; private set; }
        public List<Account> Users { get; private set; }

        [BindProperty] 
        public Booking NewBooking { get; set; }
        [BindProperty] 
        public string SearchName { get; set; }
        [BindProperty] 
        public int EditBookingID { get; set; }
        [BindProperty] 
        public Booking EditBooking { get; set; }

        private readonly BookingService _bookingService;

        //UserService til at hente subklasser
        private readonly UserService _userService;

        public string Message { get; private set; }

        public BookingListModel()
        {
            var bookingRepo = new BookingCollectionRepo(_connectionString);
            var roomRepo = new RoomCollectionRepo(_connectionString);
            var userRepo = new UserCollectionRepo(_connectionString);

            _bookingService = new BookingService(bookingRepo, roomRepo);

            //UserService som kan hente Student/Teacher/Admin
            _userService = new UserService(userRepo);
        }

        public void OnGet()
        {
            LoadData();
        }

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

        public void OnPostSearch()
        {
            LoadData();
            if (string.IsNullOrWhiteSpace(SearchName))
                return;

            string searchLower = SearchName.ToLower();
            List<Booking> filtered = new List<Booking>();

            foreach (var booking in Bookings)
            {
                string username = booking.Account?.Username;

                if (!string.IsNullOrEmpty(username) &&
                    username.ToLower().Contains(searchLower))
                {
                    filtered.Add(booking);
                }
            }

            Bookings = filtered;
        }

        public IActionResult OnPostDelete(int bookingID)
        {
            var booking = _bookingService.GetBookingById(bookingID);

            _bookingService.Delete(bookingID);

            var noteRepo = new NotificationCollectionRepo(_connectionString);
            var noteService = new NotificationService(noteRepo);

            noteService.Create(
                booking.AccountID,
                $"Din booking den {booking.BookingDate:dd-MM-yyyy} blev slettet af en administrator/lærer."
            );

            Message = "Booking slettet!";
            LoadData();
            return Page();
        }

        public IActionResult OnPostStartEdit(int bookingID)
        {
            Message = "StartEdit ramte: " + bookingID;
            EditBookingID = bookingID;
            EditBooking = _bookingService.GetBookingById(bookingID);
            LoadData();
            return Page();
        }

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

            LoadData();
            return Page();
        }

        
        private void LoadData()
        {
            Bookings = _bookingService.GetAll();

          
            foreach (var booking in Bookings)
            {
                booking.Account = _userService.GetById(booking.AccountID);
                // Nu bliver booking.Account = Student / Teacher / Administrator
            }

            var roomRepo = new RoomCollectionRepo(_connectionString);
            Rooms = roomRepo.GetAllRooms();

            var userRepo = new UserCollectionRepo(_connectionString);
            Users = userRepo.GetAllUsers();
        }
    }
}