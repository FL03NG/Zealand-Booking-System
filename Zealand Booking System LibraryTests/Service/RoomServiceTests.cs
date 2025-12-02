using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zealand_Booking_System_Library.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zealand_Booking_System_Library.Service;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Models;
using Moq;
namespace Zealand_Booking_System_Library.Service.Tests
{
    [TestClass()]
    public class RoomServiceTests
    {
        [TestMethod]
        public void AddRoom_ShouldCallRepositoryAddRoom_Once()
        {
            // Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomName = "TestRoom",
                RoomLocation = "A123",
                RoomDescription = "Test description",
                RoomType = RoomType.ClassRoom,
                HasSmartBoard = true
            };

            // Act
            service.AddRoom(room);

            // Assert: verify that AddRoom was called exactly once with the same room
            mockRepo.Verify(r => r.AddRoom(It.Is<Room>(x =>
                x.RoomName == "TestRoom" &&
                x.RoomLocation == "A123" &&
                x.RoomDescription == "Test description" &&
                x.RoomType == RoomType.ClassRoom &&
                x.HasSmartBoard == true
            )), Times.Once);
        }

        [TestMethod]
        public void AddRoom_NullRoom_ShouldThrowArgumentNullException()
        {
            // Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            // Act + Assert
            Assert.ThrowsException<ArgumentNullException>(() => service.AddRoom(null));
        }
    }
}