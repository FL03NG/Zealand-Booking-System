using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System.Pages
{
    public class NotificationsModel : PageModel
    {
        private readonly NotificationService _service;

        public List<Notification> Notifications { get; set; }

        public NotificationsModel(NotificationService service)
        {
            _service = service;
        }

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

        public IActionResult OnPostMarkRead(int notificationID)
        {
            _service.MarkAsRead(notificationID);
            return RedirectToPage();
        }
    }
}