using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Models;

namespace Zealand_Booking_System_Library.Service
{
    /// <summary>
    /// Handles business rules related to rooms.
    /// This service acts as a protective layer between the UI and the repository,
    /// ensuring that only valid room data is saved or modified.
    /// </summary>
    public class RoomService
    {
        /// <summary>
        /// Repository dependency injected to keep the service focused on rules
        /// rather than data access details.
        /// </summary>
        private readonly IRoomRepository _roomRepo;

        public RoomService(IRoomRepository roomRepo)
        {
            _roomRepo = roomRepo;
        }
        /// <summary>
        /// Returns a single room by ID, relying on the repository to handle data retrieval.
        /// Keeping this method simple avoids duplicating repository logic.
        /// </summary>
        public Room GetRoomById(int roomID)
        {
            return _roomRepo.GetRoomById(roomID);
        }
        /// <summary>
        /// Validates room data before saving it, so invalid rooms never reach the data layer.
        /// This protects the system from inconsistent or incomplete room entries.
        /// </summary>
        public void AddRoom(Room room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            if (string.IsNullOrWhiteSpace(room.RoomName))
                throw new ArgumentException("RoomName cannot be empty.");

            if (string.IsNullOrWhiteSpace(room.RoomLocation))
                throw new ArgumentException("RoomLocation cannot be empty.");

            if (!Enum.IsDefined(typeof(RoomType), room.RoomType))
                throw new ArgumentException("Invalid RoomType.");

            _roomRepo.AddRoom(room);
        }
        /// <summary>
        /// Ensures a valid ID before attempting deletion,
        /// preventing unnecessary or unsafe repository calls.
        /// </summary>
        public void DeleteRoom(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid room ID");
            _roomRepo.DeleteRoom(id);
        }
        /// <summary>
        /// Retrieves all rooms through the repository,
        /// keeping the service free from data access responsibility.
        /// </summary>
        public List<Room> GetAllRooms()
        {
            return _roomRepo.GetAllRooms();
        }
        /// <summary>
        /// Verifies that updated room data is valid before it replaces existing data,
        /// ensuring the system stays consistent over time.
        /// </summary>
        public void UpdateRoom(Room room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            if (room.RoomID <= 0)
                throw new ArgumentException("RoomID must be a positive number.");

            if (string.IsNullOrWhiteSpace(room.RoomName))
                throw new ArgumentException("RoomName cannot be empty.");

            if (string.IsNullOrWhiteSpace(room.RoomLocation))
                throw new ArgumentException("RoomLocation cannot be empty.");

            if (!Enum.IsDefined(typeof(RoomType), room.RoomType))
                throw new ArgumentException("Invalid RoomType.");
            _roomRepo.UpdateRoom(room);
        }
    }
}
