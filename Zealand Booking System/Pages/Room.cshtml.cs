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
    public class RoomModel : PageModel
    {
        private readonly string _connectionString =
           "Server=(localdb)\\MSSQLLocalDB;Database=RoomBooking;Trusted_Connection=True;TrustServerCertificate=True;";

        [BindProperty]
        public int EditRoomID { get; set; }

        [BindProperty]
        public Room EditRoom { get; set; }

        public List<Room> Room { get; set; } = new List<Room>();

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string RoomTypeFilter { get; set; } = string.Empty;

        private RoomService _roomService;

        [BindProperty]
        public Room NewRoom { get; set; } = new Room();

        public RoomModel()
        {
            RoomCollectionRepo repo = new RoomCollectionRepo(_connectionString);
            _roomService = new RoomService(repo);
        }

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

        public IActionResult OnPostStartEdit(int roomID)
        {
            EditRoomID = roomID;
            EditRoom = _roomService.GetRoomById(roomID);
            Room = _roomService.GetAllRooms();
            return Page();
        }

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