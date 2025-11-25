using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;

namespace Zealand_Booking_System.Pages.Accounts
{
    public class UserListModel : PageModel
    {
        private readonly string _connectionString =
            "Server =(localdb)\\MSSQLLocalDB;Database=RoomBooking;Trusted_Connection=True;TrustServerCertificate=True";

        public List<Account> Users { get; private set; }

        [BindProperty]
        public string SearchName { get; set; }

        [BindProperty]
        public int EditUserID { get; set; }

        [BindProperty]
        public Account EditUser { get; set; }

        public string Message { get; private set; }

        public void OnGet()
        {
            LoadData();
        }

        public void OnPostSearch()
        {
            LoadData();

            if (string.IsNullOrWhiteSpace(SearchName))
                return;

            string searchLower = SearchName.ToLower();
            List<Account> filtered = new List<Account>();

            foreach (var user in Users)
            {
                if (user.Username != null &&
                    user.Username.ToLower().Contains(searchLower))
                {
                    filtered.Add(user);
                }
            }

            Users = filtered;
        }

        public IActionResult OnPostStartEdit(int accountID)
        {
            EditUserID = accountID;

            UserCollectionRepo repo = new UserCollectionRepo(_connectionString);
            EditUser = repo.GetUserById(accountID);

            LoadData();
            return Page();
        }

        public IActionResult OnPostSaveEdit()
        {
            UserCollectionRepo repo = new UserCollectionRepo(_connectionString);

            repo.UpdateUser(EditUser);

            Message = "Bruger opdateret!";
            LoadData();

            return Page();
        }

        public IActionResult OnPostDelete(int accountID)
        {
            UserCollectionRepo repo = new UserCollectionRepo(_connectionString);

            repo.DeleteUser(accountID);

            Message = "Bruger slettet!";
            LoadData();

            return Page();
        }

        private void LoadData()
        {
            UserCollectionRepo userRepo = new UserCollectionRepo(_connectionString);
            Users = userRepo.GetAllUsers();
        }
    }
}