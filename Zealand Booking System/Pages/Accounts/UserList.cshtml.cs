using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;

namespace Zealand_Booking_System.Pages.Accounts
{
    /// <summary>
    /// PageModel responsible for displaying, searching, editing, and deleting user accounts.
    /// Encapsulates user management logic and interacts with the repository for persistence.
    /// </summary>
    public class UserListModel : PageModel
    {
        /// <summary>
        /// Connection string used for repository access.
        /// Kept private to prevent leaking database details to the UI.
        /// </summary>

        private readonly string _connectionString =
            "Server =(localdb)\\MSSQLLocalDB;Database=RoomBooking;Trusted_Connection=True;TrustServerCertificate=True";
        /// <summary>
        /// Collection of all users to be displayed in the UI.
        /// </summary>
        public List<Account> Users { get; private set; }
        /// <summary>
        /// Search term bound from the UI to filter users by username.
        /// </summary>
        [BindProperty]
        public string SearchName { get; set; }
        /// <summary>
        /// ID of the user currently being edited.
        /// </summary>
        [BindProperty]
        public int EditUserID { get; set; }
        /// <summary>
        /// Account object used for editing a user.
        /// </summary>
        [BindProperty]
        public Account EditUser { get; set; }
        /// <summary>
        /// Status message displayed after an operation like edit or delete.
        /// </summary>
        public string Message { get; private set; }
        /// <summary>
        /// Handles initial GET requests and loads all users.
        /// </summary>
        public void OnGet()
        {
            LoadData();
        }
        /// <summary>
        /// Filters the user list based on the provided search term.
        /// Search is case-insensitive.
        /// </summary>
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
        /// <summary>
        /// Initializes the editing state for a specific user.
        /// Populates the EditUser property and reloads the user list.
        /// </summary>
        public IActionResult OnPostStartEdit(int accountID)
        {
            EditUserID = accountID;
            UserCollectionRepo repo = new UserCollectionRepo(_connectionString);
            EditUser = repo.GetById(accountID);
            LoadData();
            return Page();
        }
        /// <summary>
        /// Saves changes made to an existing user.
        /// Updates the repository and reloads the user list.
        /// </summary>
        public IActionResult OnPostSaveEdit()
        {
            UserCollectionRepo repo = new UserCollectionRepo(_connectionString);
            repo.UpdateUser(EditUser);
            Message = "Bruger opdateret!";
            LoadData();
            return Page();
        }
        /// <summary>
        /// Deletes a user by ID.
        /// Updates the repository and reloads the user list with a status message.
        /// </summary>
        public IActionResult OnPostDelete(int accountID)
        {
            UserCollectionRepo repo = new UserCollectionRepo(_connectionString);
            repo.DeleteUser(accountID);
            Message = "Bruger slettet!";
            LoadData();
            return Page();
        }
        /// <summary>
        /// Loads all users from the repository.
        /// Centralized to reduce duplication across handlers.
        /// </summary>
        private void LoadData()
        {
            UserCollectionRepo userRepo = new UserCollectionRepo(_connectionString);
            Users = userRepo.GetAllUsers();
        }
    }
}