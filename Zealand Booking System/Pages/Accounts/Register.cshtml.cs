using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System.Pages.Accounts
{
    public class RegisterModel : PageModel
    {
        private readonly UserService _userService;

        public RegisterModel(UserService service)
        {
            _userService = service;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string Role { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Udfyld alle felter.";
                return Page();
            }

            // Evt hashing, men vi bruger ren tekst nu
            var newUser = Role switch
            {
                "Administrator" => new Administrator(),
                "Teacher" => new Teacher(),
                "Student" => new Student(),
                _ => new Zealand_Booking_System_Library.Models.Account()
            };

            newUser.Username = Username;
            newUser.PasswordHash = Password;

            try
            {
                _userService.Create(newUser, Role);
                SuccessMessage = "Bruger oprettet!";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Fejl: {ex.Message}";
            }

            return Page();
        }

    }
}
