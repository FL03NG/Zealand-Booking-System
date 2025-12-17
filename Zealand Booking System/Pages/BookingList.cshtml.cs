using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
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
            BookingCollectionRepo bookingRepo = new BookingCollectionRepo(_connectionString);
            RoomCollectionRepo roomRepo = new RoomCollectionRepo(_connectionString);
            UserCollectionRepo userRepo = new UserCollectionRepo(_connectionString);

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
                Message = "Booking created!";
            }
            catch (Exception ex)
            {
                Message = "Error: " + ex.Message;
            }

            LoadData();
        }

        public void OnPostSearch()
        {
            LoadData();
            if (string.IsNullOrWhiteSpace(SearchName))
            {
                return;
            }

            string searchLower = SearchName.ToLower();
            List<Booking> filtered = new List<Booking>();

            foreach (Booking booking in Bookings)
            {
                string username = booking.Account != null ? booking.Account.Username : null;

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
            try
            {
                string role = HttpContext.Session.GetString("Role");

                Booking booking = _bookingService.GetBookingById(bookingID);

                _bookingService.Delete(bookingID, role);

                NotificationCollectionRepo noteRepo = new NotificationCollectionRepo(_connectionString);
                NotificationService noteService = new NotificationService(noteRepo);

                noteService.Create(
                    booking.AccountID,
                    $"Your booking at {booking.BookingDate:dd-MM-yyyy} has been deleted."
                );

                Message = "Booking deleted!";
            }
            catch (Exception ex)
            {
                // Fx hvis Teacher bryder 3-dages-reglen
                Message = "Error: " + ex.Message;
            }

            LoadData();
            return Page();
        }

        public IActionResult OnPostStartEdit(int bookingID)
        {
            Message = "Editing startet";
            EditBookingID = bookingID;
            EditBooking = _bookingService.GetBookingById(bookingID);
            LoadData();
            return Page();
        }

        public IActionResult OnPostSaveEdit()
        {
            try
            {
                string role = HttpContext.Session.GetString("Role");

                _bookingService.Update(EditBooking, role);
                Message = "Booking has been editet!";
            }
            catch (Exception ex)
            {
                Message = "Error during editing: " + ex.Message;
                LoadData();
                return Page();
            }

            LoadData();
            return Page();
        }

        private void LoadData()
        {
            Bookings = _bookingService.GetAll();
            RoomCollectionRepo roomRepo = new RoomCollectionRepo(_connectionString);
            Rooms = roomRepo.GetAllRooms();

            foreach (Booking booking in Bookings)
            {
                booking.Account = _userService.GetById(booking.AccountID);
                booking.Room = roomRepo.GetRoomById(booking.RoomID);
                // Nu bliver booking.Account = Student / Teacher / Administrator
            }

            

            //UserCollectionRepo userRepo = new UserCollectionRepo(_connectionString);
            //Users = userRepo.GetAllUsers();
        }
    }
}
