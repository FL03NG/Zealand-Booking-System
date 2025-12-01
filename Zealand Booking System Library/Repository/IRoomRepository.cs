using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Models;

namespace Zealand_Booking_System_Library.Repository
{
    public interface IRoomRepository
    {
        public void AddRoom(Room room);
        public Room GetRoomById(int roomID);
        public void DeleteRoom(int id);
        public List<Room> GetAllRooms();
        public void UpdateRoom(Room room);

    }
}

