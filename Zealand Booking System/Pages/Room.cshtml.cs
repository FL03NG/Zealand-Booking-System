using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zealand_Booking_System_Library.Models;

namespace Zealand_Booking_System.Pages
{
    public class RoomModel : PageModel
    {
        private readonly string _connectionString =
           "Server=(localdb)\\MSSQLLocalDB;Database=RoomBooking;Trusted_Connection=True;TrustServerCertificate=True;";

        // Liste som indeholder alle beboere
        public List<Room> Resident { get; set; } = new List<Resident>();

        // Service som håndterer CRUD-operationer for beboere
        private ResidentService _residentService;

        // Property som bruges til at modtage nye beboerdata fra formularen
        [BindProperty]
        public Resident NewResident { get; set; } = new Resident();

        // Constructor – initialiserer repository og service så vi kan forbinde til databasen
        public ResidentGridModel()
        {
            // Opretter repository og sender det videre til service-laget
            ResidentCollectionRepo repo = new ResidentCollectionRepo(_connectionString);
            _residentService = new ResidentService(repo);
        }

        // GET-metode – kaldes når siden hentes første gang
        // Henter alle eksisterende beboere fra databasen
        public void OnGet()
        {
            Resident = _residentService.GetAll();
        }

        // POST-metode – kaldes når brugeren indsender formularen for at oprette en ny beboer
        public IActionResult OnPost()
        {
            try
            {
                // Forsøger at tilføje den nye beboer til databasen
                _residentService.Add(NewResident);

                // Viser besked på siden hvis oprettelsen lykkes
                TempData["Message"] = "Beboer oprettet!";
            }
            catch (System.Exception ex)
            {
                // Hvis der sker en fejl, vises en fejlbesked til brugeren
                TempData["Message"] = "Fejl under oprettelse: " + ex.Message;
            }

            // Genindlæser siden så brugeren ser den opdaterede liste
            return RedirectToPage();
        }

        // POST-metode – kaldes når brugeren trykker på "Slet" knappen
        // Denne metode modtager et ID på den beboer der skal slettes
        public IActionResult OnPostDelete(int residentID)
        {
            try
            {
                // Forsøger at slette beboeren fra databasen
                _residentService.Delete(residentID);

                // Viser besked hvis sletningen lykkes
                TempData["Message"] = "Beboer slettet!";
            }
            catch (System.Exception ex)
            {
                // Hvis der sker fejl, vis besked på siden
                TempData["Message"] = "Fejl under sletning: " + ex.Message;
            }

            // Genindlæser siden for at vise opdateret liste
            return RedirectToPage();
        }
    }
}
