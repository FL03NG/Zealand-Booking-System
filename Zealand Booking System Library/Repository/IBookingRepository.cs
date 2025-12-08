using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Models;

namespace Zealand_Booking_System_Library.Repository
{
    /// <summary>
    /// Defines the contract for all booking repository implementations.
    ///
    /// Responsibility:
    /// - Specifies which operations the application requires for working with bookings.
    /// - Ensures that services and Razor Pages do not depend on a specific database technology.
    ///
    /// Why this interface exists:
    /// - Supports dependency injection and loose coupling.
    /// - Allows the system to switch between different storage solutions 
    ///   (SQL, in-memory, mock repository for unit tests) without changing logic elsewhere.
    /// - Improves testability by allowing fake or mocked repositories.
    /// </summary>
    public interface IBookingRepository
    {
        /// <summary>
        /// Creates a new booking and saves it in the storage layer.
        /// Why:
        /// - Central operation needed by Create Booking flow.
        /// </summary>
        public void Add(Booking booking);

        /// <summary>
        /// Retrieves a single booking by its unique ID, including related data.
        /// Why:
        /// - Allows pages and services to load detailed booking information when editing or viewing.
        /// </summary>
        public Booking GetBookingById(int bookingID);

        /// <summary>
        /// Deletes a booking using its ID.
        /// Why:
        /// - Needed for canceling bookings and managing data cleanup.
        /// </summary>
        public void Delete(int id);

        /// <summary>
        /// Loads all bookings from the storage.
        /// Why:
        /// - Used by admin pages and booking lists.
        /// - Provides a central place to define how bookings are fetched.
        /// </summary>
        public List<Booking> GetAll();

        /// <summary>
        /// Updates an existing booking.
        /// Why:
        /// - Allows users or admins to modify reservation details.
        /// </summary>
        public void Update(Booking booking);
    }
}
