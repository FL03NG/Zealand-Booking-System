using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;

namespace Zealand_Booking_System_Library.Service
{
    public class NotificationService
    {
        private readonly INotificationRepository _repo;

        public NotificationService(INotificationRepository repo)
        {
            _repo = repo;
        }

        public void Create(int accountId, string message)
        {
            _repo.Add(new Notification
            {
                AccountID = accountId,
                Message = message
            });
        }

        public List<Notification> GetUnread(int accountId)
        {
            return _repo.GetUnread(accountId);
        }

        public void MarkAsRead(int id)
        {
            _repo.MarkAsRead(id);
        }
    }
}
