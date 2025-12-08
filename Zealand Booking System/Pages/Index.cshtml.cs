using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Zealand_Booking_System.Pages
{
    /// <summary>
    /// PageModel for the landing page of the Zealand Booking System.
    ///
    /// Responsibility:
    /// - Handles requests to the front page (Index.cshtml).
    /// - Provides a place for logging or future logic if needed.
    ///
    /// Why this class exists:
    /// - Razor Pages require a PageModel to follow the page lifecycle.
    /// - Allows the project to be extended later without modifying the UI file.
    /// </summary>
    public class IndexModel : PageModel
    {
        /// <summary>
        /// Logger injected by ASP.NET Core.
        /// Useful for debugging, analytics or tracking page access.
        /// </summary>
        private readonly ILogger<IndexModel> _logger;

        /// <summary>
        /// Constructor uses dependency injection to provide logging functionality.
        /// </summary>
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Called when the page is accessed with GET.
        ///
        /// Current behavior:
        /// - No logic needed for this page (purely display content).
        ///
        /// Why:
        /// - Index.cshtml dynamically handles UI based on session data,
        ///   so the PageModel remains simple and clean.
        /// </summary>
        public void OnGet()
        {
            
        }
    }
}
