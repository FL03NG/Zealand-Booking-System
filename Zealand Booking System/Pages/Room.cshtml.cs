using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Service;

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

        // Liste som indeholder alle lokaler
        public List<Room> Room { get; set; } = new List<Room>();

        // Property til søgning
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; } = string.Empty;

        // Service som håndterer CRUD-operationer for lokaler
        private RoomService _roomService;

        // Property som bruges til at modtage nye lokaledata fra formularen
        [BindProperty]
        public Room NewRoom { get; set; } = new Room();

        // Constructor – initialiserer repository og service så vi kan forbinde til databasen
        public RoomModel()
        {
            RoomCollectionRepo repo = new RoomCollectionRepo(_connectionString);
            _roomService = new RoomService(repo);
        }

        // GET-metode – kaldes når siden hentes første gang
        public void OnGet()
        {
            List<Room> allRooms = _roomService.GetAllRooms();

            if (!string.IsNullOrEmpty(SearchString))
            {
                List<Room> filtered = new List<Room>();

                foreach (Room r in allRooms)
                {
                    if (r.RoomName != null &&
                        r.RoomName.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        filtered.Add(r);
                    }
                }

                Room = filtered;
            }
            else
            {
                Room = allRooms;
            }
        }

        // POST – opret nyt lokale
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

        // POST – slet lokale
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

        // POST – start redigering
        public IActionResult OnPostStartEdit(int roomID)
        {
            EditRoomID = roomID;
            EditRoom = _roomService.GetRoomById(roomID);
            Room = _roomService.GetAllRooms(); // reload list
            return Page();
        }

        // POST – gem redigering
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
