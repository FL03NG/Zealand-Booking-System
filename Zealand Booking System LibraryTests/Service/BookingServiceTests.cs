using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System_Library.Service.Tests
{
    [TestClass]
    public class BookingServiceTests
    {
        [TestMethod]
        public void Add_ValidBooking_ShouldCallRepositoryAdd_Once()
        {
            // Arrange
            var bookingRepoMock = new Mock<IBookingRepository>();
            var roomRepoMock = new Mock<IRoomRepository>();

            var room = new Room
            {
                RoomID = 1,
                RoomName = "A101",
                RoomType = RoomType.ClassRoom,
                RoomDescription = "Test",
                RoomLocation = "A-Bygning"
            };

            // Når service spørger efter rummet, får den dette
            roomRepoMock
                .Setup(r => r.GetRoomById(1))
                .Returns(room);

            // Ingen eksisterende bookinger
            var existingBookings = new List<Booking>();
            bookingRepoMock
                .Setup(b => b.GetAll())
                .Returns(existingBookings);

            var service = new BookingService(bookingRepoMock.Object, roomRepoMock.Object);

            var booking = new Booking
            {
                RoomID = 1,
                AccountID = 1,
                BookingDate = DateTime.Today,
                TimeSlot = TimeSlot.Slot08_10,
                BookingDescription = "Gruppearbejde"
            };

            // Act
            service.Add(booking);

            // Assert – der må ikke være kastet exception,
            // og Add skal være kaldt præcis én gang med samme booking
            bookingRepoMock.Verify(b => b.Add(It.Is<Booking>(x =>
                x.RoomID == booking.RoomID &&
                x.AccountID == booking.AccountID &&
                x.BookingDate == booking.BookingDate &&
                x.TimeSlot == booking.TimeSlot &&
                x.BookingDescription == booking.BookingDescription
            )), Times.Once);
        }

        [TestMethod]
        public void Add_SameUserSameDaySameSlot_ShouldThrowException()
        {
            // Arrange
            var bookingRepoMock = new Mock<IBookingRepository>();
            var roomRepoMock = new Mock<IRoomRepository>();

            var room = new Room
            {
                RoomID = 2,
                RoomName = "Møderum 1",
                RoomType = RoomType.MeetingRoom,
                RoomDescription = "Møde",
                RoomLocation = "B-Bygning"
            };

            roomRepoMock
                .Setup(r => r.GetRoomById(2))
                .Returns(room);

            var dato = DateTime.Today;

            // Eksisterende booking for samme bruger, samme dag og slot
            var existingBooking = new Booking
            {
                RoomID = 2,
                AccountID = 1,
                BookingDate = dato,
                TimeSlot = TimeSlot.Slot10_12
            };

            var existingBookings = new List<Booking> { existingBooking };

            // Når BookingService kalder GetAll(), får den denne liste
            bookingRepoMock
                .Setup(b => b.GetAll())
                .Returns(existingBookings);

            var service = new BookingService(bookingRepoMock.Object, roomRepoMock.Object);

            // Ny booking med samme bruger, samme dato, samme tidsrum
            var newBooking = new Booking
            {
                RoomID = 2,
                AccountID = 1,
                BookingDate = dato,
                TimeSlot = TimeSlot.Slot10_12
            };

            // Act + Assert
            var ex = Assert.ThrowsException<Exception>(() => service.Add(newBooking));

            // Her bruger vi præcis den tekst du har i BookingService:
            // "Du har allerede en booking i dette tidsrum."
            Assert.AreEqual("Du har allerede en booking i dette tidsrum.", ex.Message);

            // Og vi kan evt. også tjekke at Add IKKE blev kaldt
            bookingRepoMock.Verify(b => b.Add(It.IsAny<Booking>()), Times.Never);
        }
    }
}
