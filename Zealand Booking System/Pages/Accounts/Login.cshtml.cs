using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System.Pages.Accounts
{
    /// <summary>
    /// PageModel responsible for handling user login.
    /// Validates credentials using UserService and sets session variables for authenticated users.
    /// </summary>
    public class LoginModel : PageModel
    {
        /// <summary>
        /// Service responsible for managing user authentication and retrieval.
        /// </summary>
        private readonly UserService _userService;
        /// <summary>
        /// Initializes a new instance of the LoginModel with the given UserService.
        /// </summary>
        /// <param name="userService">The user service for authentication.</param>
        public LoginModel(UserService userService)
        {
            _userService = userService;
        }
        /// <summary>
        /// Username entered by the user in the login form.
        /// </summary>
        [BindProperty]
        public string Username { get; set; }
        /// <summary>
        /// Password entered by the user in the login form.
        /// </summary>
        [BindProperty]
        public string Password { get; set; }
        /// <summary>
        /// Error message to display if login fails.
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// Handles POST requests from the login form.
        /// Validates credentials using UserService. 
        /// If successful, stores user information in session and redirects to Index.
        /// Sets an error message if login fails.
        /// </summary>
        /// <returns>Redirects to Index page on success, or reloads the login page on failure.</returns>
        public IActionResult OnPost()
        {
            var user = _userService.Login(Username, Password);
            if (user == null)
            {
                ErrorMessage = "Forkert username eller password.";
                return Page();
            }
            // Persist user info in session for later authentication checks
            HttpContext.Session.SetInt32("AccountID", user.AccountID);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);
            return RedirectToPage("/Index");
        }
    }
}
