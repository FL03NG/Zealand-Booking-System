using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Models;

namespace Zealand_Booking_System_Library.Repository
{
    public interface IBookingRepository
    {
        public void Add(Booking booking);
        public void Delete(int id);
        public List<Booking> GetAll();
        public void Update(Booking booking);
    }
}
