using System.Diagnostics;
using System.Reflection.PortableExecutable;
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
            // Opretter repository og sender det videre til service-laget
            RoomCollectionRepo repo = new RoomCollectionRepo(_connectionString);
            _roomService = new RoomService(repo);
        }

        // GET-metode – kaldes når siden hentes første gang
        // Henter alle eksisterende lokaler fra databasen
        public void OnGet()
        {
            var allRooms = _roomService.GetAllRooms();
            if (!string.IsNullOrEmpty(SearchString))
            {
                // Filtrerer lokaler baseret på søgeteksten
                Room = allRooms
                    .Where(r => r.RoomName.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                Room = allRooms;
            }
        }

        // POST-metode – kaldes når brugeren indsender formularen for at oprette et nyt lokale
        public IActionResult OnPost()
        {
            try
            {
                // Forsøger at tilføje det nye lokale til databasen
                _roomService.AddRoom(NewRoom);

                // Viser besked hvis oprettelsen lykkes
                Debug.WriteLine("Lokale oprettet!");
            }
            catch (System.Exception ex)
            {
                // Hvis der sker en fejl, vises en fejlbesked
                Debug.WriteLine("Fejl under oprettelse: " + ex.Message);
            }

            // Genindlæser siden så brugeren ser den opdaterede liste
            return RedirectToPage();
        }

        // POST-metode – kaldes når brugeren trykker på "Slet" knappen
        // Denne metode modtager et ID på det lokale der skal slettes
        public IActionResult OnPostDelete(int roomID)
        {
            try
            {
                _roomService.DeleteRoom(roomID);
                TempData["Message"] = "Lokale slettet!";
            }
            catch (System.Exception ex)
            {
                TempData["Message"] = "Fejl under sletning: " + ex.Message;
            }

            return RedirectToPage();
        }

        public IActionResult OnPostStartEdit(int roomID)
        {
            EditRoomID = roomID;
            EditRoom = _roomService.GetRoomById(roomID);
            Room = _roomService.GetAllRooms(); // reload list
            return Page();
        }
       public IActionResult OnPostSaveEdit()
{
    try
    {
        _roomService.UpdateRoom(EditRoom);
        TempData["Message"] = "Lokalet er ændret!";
    }
    catch (System.Exception ex)
    {
        TempData["Message"] = "Fejl under redigering: " + ex.Message;
    }

    return RedirectToPage(); // reload
}


    }
}
