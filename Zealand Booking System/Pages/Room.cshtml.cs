using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Service;
using System.Linq;

namespace Zealand_Booking_System.Pages
{
    /// <summary>
    /// PageModel responsible for managing rooms in the system UI.
    /// It coordinates user input, filtering, and edit state,
    /// while delegating business logic and persistence to the service layer.
    /// </summary>
    public class RoomModel : PageModel
    {
        /// <summary>
        /// Connection string used to configure repository access.
        /// Database concerns are still abstracted behind the repository layer.
        /// </summary>
        private readonly string _connectionString =
           "Server=(localdb)\\MSSQLLocalDB;Database=RoomBooking;Trusted_Connection=True;TrustServerCertificate=True;";
        /// <summary>
        /// Identifies which room is currently being edited,
        /// allowing the UI to switch between view and edit modes.
        /// </summary>
        [BindProperty]
        public int EditRoomID { get; set; }
        /// <summary>
        /// Holds the room currently being edited.
        /// This prevents changes from being persisted until explicitly saved.
        /// </summary>
        [BindProperty]
        public Room EditRoom { get; set; }
        /// <summary>
        /// Collection of rooms displayed in the UI.
        /// Filtering is applied server-side to keep logic centralized.
        /// </summary>
        public List<Room> Rooms { get; set; } = new List<Room>();
        /// <summary>
        /// Search text used to filter rooms by name.
        /// Supports GET binding so filters persist in the URL.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; } = string.Empty;
        /// <summary>
        /// Selected room type filter.
        /// Also bound via GET to maintain state across reloads.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string RoomTypeFilter { get; set; } = string.Empty;
        /// <summary>
        /// Service responsible for room-related business rules.
        /// </summary>
        private RoomService _roomService;
        /// <summary>
        /// Holds data for creating a new room.
        /// Separated from EditRoom to avoid shared state.
        /// </summary>
        [BindProperty]
        public Room NewRoom { get; set; } = new Room();
        /// <summary>
        /// Initializes the PageModel and configures service dependencies.
        /// Repository creation is kept out of the Razor view.
        /// </summary>
        public RoomModel()
        {
            RoomCollectionRepo repo = new RoomCollectionRepo(_connectionString);
            _roomService = new RoomService(repo);
        }
        /// <summary>
        /// Loads all rooms and applies optional filtering
        /// based on search text and selected room type.
        /// </summary>
        public void OnGet()
        {
            var allRooms = _roomService.GetAllRooms();
            if (!string.IsNullOrEmpty(SearchString))
            {
                allRooms = allRooms
                    .Where(r => !string.IsNullOrEmpty(r.RoomName) &&
                                r.RoomName.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            if (!string.IsNullOrEmpty(RoomTypeFilter))
            {
                allRooms = allRooms
                    .Where(r => r.RoomType.ToString() == RoomTypeFilter)
                    .ToList();
            }
            Room = allRooms;
        }
        /// <summary>
        /// Handles creation of a new room.
        /// All validation and persistence rules are enforced by the service layer.
        /// </summary>
        public IActionResult OnPost()
        {
            try
            {
                _roomService.AddRoom(NewRoom);
                Debug.WriteLine("Lokale oprettet!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fejl under oprettelse: " + ex.Message);
            }
            return RedirectToPage();
        }
        /// <summary>
        /// Deletes a room by ID and redirects to avoid duplicate submissions.
        /// </summary>
        public IActionResult OnPostDelete(int roomID)
        {
            try
            {
                _roomService.DeleteRoom(roomID);
                TempData["Message"] = "Lokale slettet!";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Fejl under sletning: " + ex.Message;
            }
            return RedirectToPage();
        }
        /// <summary>
        /// Activates edit mode for a specific room and reloads data
        /// to ensure the latest version is edited.
        /// </summary>
        public IActionResult OnPostStartEdit(int roomID)
        {
            EditRoomID = roomID;
            EditRoom = _roomService.GetRoomById(roomID);
            Room = _roomService.GetAllRooms();
            return Page();
        }
        /// <summary>
        /// Saves changes made to a room.
        /// Updates are passed through the service layer
        /// to enforce business rules consistently.
        /// </summary>
        public IActionResult OnPostSaveEdit()
        {
            try
            {
                _roomService.UpdateRoom(EditRoom);
                TempData["Message"] = "Lokalet er ændret!";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Fejl under redigering: " + ex.Message;
            }
            return RedirectToPage();
        }
    }
}