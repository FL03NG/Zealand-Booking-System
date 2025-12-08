using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System.Pages.Accounts
{
    /// <summary>
    /// PageModel responsible for registering new users.
    /// Handles input from the registration form, creates the appropriate user type,
    /// and interacts with the UserService to persist the new account.
    /// </summary>
    public class RegisterModel : PageModel
    {
        /// <summary>
        /// Service responsible for managing user accounts.
        /// </summary>
        private readonly UserService _userService;
        /// <summary>
        /// Initializes a new instance of the RegisterModel with a given UserService.
        /// </summary>
        /// <param name="service">The user service for managing accounts.</param>
        public RegisterModel(UserService service)
        {
            _userService = service;
        }
        /// <summary>
        /// Username entered by the user in the registration form.
        /// </summary>
        [BindProperty]
        public string Username { get; set; }
        /// <summary>
        /// Password entered by the user in the registration form.
        /// </summary>
        [BindProperty]
        public string Password { get; set; }
        /// <summary>
        /// Selected role for the new user.
        /// </summary>
        [BindProperty]
        public string Role { get; set; }
        /// <summary>
        /// Error message to display in case of invalid input or failure.
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// Success message to display after successful user creation.
        /// </summary>
        public string SuccessMessage { get; set; }
        /// <summary>
        /// Handles POST requests from the registration form.
        /// Validates input, creates the appropriate user type, and persists it using UserService.
        /// Sets success or error messages accordingly.
        /// </summary>
        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Udfyld alle felter.";
                return Page();
            }
            // Instantiate user based on the selected role
            var newUser = Role switch
            {
                "Administrator" => new Administrator(),
                "Teacher" => new Teacher(),
                "Student" => new Student(),
                _ => new Zealand_Booking_System_Library.Models.Account()
            };
            newUser.Username = Username;
            newUser.PasswordHash = Password; // Note: Password stored as plain text for now
            try
            {
                _userService.Create(newUser, Role);
                SuccessMessage = "Bruger oprettet!";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Fejl: {ex.Message}";
            }
            return RedirectToPage("/Accounts/Login");
        }
    }
}
