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
    /// <summary>
    /// PageModel responsible for displaying and managing the booking list.
    /// Supports:
    /// - Loading bookings with related room/user data
    /// - Creating a booking
    /// - Searching bookings by username
    /// - Deleting bookings (with role-based rules) and creating notifications
    /// - Editing bookings (start edit + save edit)
    ///
    /// The page keeps UI concerns here, while business rules (e.g. 3-day rule, double booking)
    /// are delegated to the service layer.
    /// </summary>
    public class BookingListModel : PageModel
    {
        /// <summary>
        /// Connection string used to configure repository access.
        /// Persistence details remain encapsulated within repositories.
        /// </summary>
        private readonly string _connectionString =
            "Server =(localdb)\\MSSQLLocalDB;Database=RoomBooking;Trusted_Connection=True;TrustServerCertificate=True";

        /// <summary>
        /// All bookings shown in the list.
        /// Can be replaced with a filtered list during search.
        /// </summary>
        public List<Booking> Bookings { get; private set; }

        /// <summary>
        /// List of rooms used for dropdowns / display.
        /// Loaded from the room repository.
        /// </summary>
        public List<Room> Rooms { get; private set; }

        /// <summary>
        /// List of users used for dropdowns / display.
        /// Loaded from the user repository.
        /// </summary>
        public List<Account> Users { get; private set; }

        /// <summary>
        /// Booking bound from the "create booking" form.
        /// </summary>
        [BindProperty]
        public Booking NewBooking { get; set; }

        /// <summary>
        /// Search text bound from the search form.
        /// Used to filter bookings by username.
        /// </summary>
        [BindProperty]
        public string SearchName { get; set; }

        /// <summary>
        /// The booking id currently being edited.
        /// Used to toggle edit mode in the UI.
        /// </summary>
        [BindProperty]
        public int EditBookingID { get; set; }

        /// <summary>
        /// The booking being edited.
        /// Bound from the edit form when saving.
        /// </summary>
        [BindProperty]
        public Booking EditBooking { get; set; }

        // ----------------------------
        // Services
        // ----------------------------

        /// <summary>
        /// Service responsible for booking logic and rules (create/update/delete/availability).
        /// Keeps business logic out of the UI layer.
        /// </summary>
        private readonly BookingService _bookingService;

        /// <summary>
        /// Service responsible for retrieving users as the correct subtype
        /// (Student / Teacher / Administrator) based on repository data.
        /// </summary>
        private readonly UserService _userService;

        /// <summary>
        /// Status message shown on the page after actions (create/edit/delete/search errors).
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Initializes repositories and composes required services for this page.
        /// </summary>
        public BookingListModel()
        {
            BookingCollectionRepo bookingRepo = new BookingCollectionRepo(_connectionString);
            RoomCollectionRepo roomRepo = new RoomCollectionRepo(_connectionString);
            UserCollectionRepo userRepo = new UserCollectionRepo(_connectionString);

            _bookingService = new BookingService(bookingRepo, roomRepo);

            // UserService can return the proper subtype: Student/Teacher/Admin
            _userService = new UserService(userRepo);
        }

        /// <summary>
        /// Default GET handler.
        /// Loads all data required for the list view.
        /// </summary>
        public void OnGet()
        {
            LoadData();
        }

        /// <summary>
        /// Handles create booking form submission.
        /// Delegates booking validation/rules to the service layer.
        /// </summary>
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

        /// <summary>
        /// Handles searching bookings by username.
        /// Loads the full dataset and then filters the bookings list in-memory.
        /// </summary>
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
                // Account is attached in LoadData()
                string username = booking.Account != null ? booking.Account.Username : null;

                if (!string.IsNullOrEmpty(username) &&
                    username.ToLower().Contains(searchLower))
                {
                    filtered.Add(booking);
                }
            }

            Bookings = filtered;
        }

        /// <summary>
        /// Deletes a booking.
        /// Uses the current session role to enforce role-based rules in the service layer
        /// (e.g. teachers might be limited by the 3-day rule).
        /// Also creates a notification for the booking owner after deletion.
        /// </summary>
        /// <param name="bookingID">Id of the booking to delete.</param>
        public IActionResult OnPostDelete(int bookingID)
        {
            try
            {
                string role = HttpContext.Session.GetString("Role");

                // Retrieve booking before deletion (needed for notification message)
                Booking booking = _bookingService.GetBookingById(bookingID);

                _bookingService.Delete(bookingID, role);

                // Create notification after successful deletion
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
                // Example: Teacher violates the 3-day rule
                Message = "Error: " + ex.Message;
            }

            LoadData();
            return Page();
        }

        /// <summary>
        /// Starts editing mode for a specific booking.
        /// Loads the booking into EditBooking so the UI can render inputs with existing values.
        /// </summary>
        public IActionResult OnPostStartEdit(int bookingID)
        {
            Message = "Editing startet";
            EditBookingID = bookingID;
            EditBooking = _bookingService.GetBookingById(bookingID);

            LoadData();
            return Page();
        }

        /// <summary>
        /// Saves edits for the selected booking.
        /// Uses the current session role to enforce role-based rules in the service layer.
        /// </summary>
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

        /// <summary>
        /// Loads bookings, users and rooms for the page.
        /// Also attaches each booking's Account as the correct subtype (Student/Teacher/Admin)
        /// so the UI can show usernames/roles correctly.
        /// </summary>
        private void LoadData()
        {
            // Load bookings via service (business layer)
            Bookings = _bookingService.GetAll();
            RoomCollectionRepo roomRepo = new RoomCollectionRepo(_connectionString);
            Rooms = roomRepo.GetAllRooms();

            // Attach Account object for each booking so the UI can show username etc.
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
