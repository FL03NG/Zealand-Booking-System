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

        public string Message { get; private set; }

        public UserListModel()
        {
            // Tom konstruktør – vi bruger bare connection string direkte
        }

        public void OnGet()
        {
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
            List<Account> filtered = new List<Account>();

            if (Users == null)
            {
                return;
            }

            for (int i = 0; i < Users.Count; i++)
            {
                Account user = Users[i];

                if (user.Username != null)
                {
                    string usernameLower = user.Username.ToLower();

                    if (usernameLower.Contains(searchLower))
                    {
                        filtered.Add(user);
                    }
                }
            }

            Users = filtered;
        }

        private void LoadData()
        {
            UserCollectionRepo userRepo = new UserCollectionRepo(_connectionString);
            Users = userRepo.GetAllUsers();
        }
    }
}
