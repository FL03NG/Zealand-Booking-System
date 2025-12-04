using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System.Pages.Accounts
{
    public class LoginModel : PageModel
    {
        private readonly UserService _userService;

        public LoginModel(UserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            var user = _userService.Login(Username, Password);

            if (user == null)
            {
                ErrorMessage = "Forkert username eller password.";
                return Page();
            }

            // Save session
            HttpContext.Session.SetInt32("AccountID", user.AccountID);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);

            return RedirectToPage("/Index");
        }
    }
}
