using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System.Pages
{
    /// <summary>
    /// PageModel responsible for displaying and updating user notifications.
    ///
    /// Responsibility:
    /// - Loads unread notifications for the logged-in user.
    /// - Handles user actions such as marking a notification as read.
    ///
    /// Why this class exists:
    /// - To separate UI logic from business logic (NotificationService).
    /// - To keep Razor Pages clean and maintainable.
    /// </summary>
    public class NotificationsModel : PageModel
    {
        /// <summary>
        /// Service used to access notification operations such as getting unread messages
        /// and marking them as read.
        /// </summary>
        private readonly NotificationService _service;

        /// <summary>
        /// List of unread notifications to be displayed on the page.
        /// Bound to the Razor Page for rendering.
        /// </summary>
        public List<Notification> Notifications { get; set; }

        /// <summary>
        /// Constructor uses dependency injection to provide the notification service.
        /// </summary>
        public NotificationsModel(NotificationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Loads unread notifications when the page is accessed with GET.
        ///
        /// Why:
        /// - Ensures users always see the most up-to-date list of notifications.
        /// - If the user is not logged in, simply provide an empty list.
        /// </summary>
        public void OnGet()
        {
            int? accountId = HttpContext.Session.GetInt32("AccountID");

            if (accountId.HasValue)
            {
                Notifications = _service.GetUnread(accountId.Value);
            }
            else
            {
                Notifications = new List<Notification>();
            }
        }

        /// <summary>
        /// Handles the action of marking a single notification as read.
        ///
        /// Why:
        /// - Makes the update explicit and secure through a POST request.
        /// - Redirects back to the page so the UI updates immediately.
        /// </summary>
        public IActionResult OnPostMarkRead(int notificationID)
        {
            _service.MarkAsRead(notificationID);
            return RedirectToPage();
        }
    }
}
