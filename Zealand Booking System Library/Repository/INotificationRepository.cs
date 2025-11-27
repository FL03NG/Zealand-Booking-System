using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Models;

namespace Zealand_Booking_System_Library.Repository
{
    public interface INotificationRepository
    {
        void Add(Notification notification);
        List<Notification> GetUnread(int accountId);
        void MarkAsRead(int notificationId);
    }
}

