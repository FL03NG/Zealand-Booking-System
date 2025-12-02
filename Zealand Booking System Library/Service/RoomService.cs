using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Models;

namespace Zealand_Booking_System_Library.Service
{
    public class RoomService
    {
        private readonly IRoomRepository _roomRepo;

        public RoomService(IRoomRepository roomRepo)
        {
            _roomRepo = roomRepo;
        }
        public Room GetRoomById(int roomID)
        {
            return _roomRepo.GetRoomById(roomID);
        }
        public void AddRoom(Room room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            _roomRepo.AddRoom(room);
        }

        public void DeleteRoom(int id)
        {
            _roomRepo.DeleteRoom(id);
        }

        public List<Room> GetAllRooms()
        {
            return _roomRepo.GetAllRooms();
        }
        public void UpdateRoom(Room room)
        {
            _roomRepo.UpdateRoom(room);
        }
    }
}
