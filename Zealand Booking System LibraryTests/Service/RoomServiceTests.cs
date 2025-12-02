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
        //--------------------------------------AddRoom---------------------
        [TestMethod]
        public void AddRoom_ShouldCallRepositoryAddRoom_Once() //happypath
        {
            // Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomName = null,
                RoomLocation = "A123",
                RoomDescription = "Test description",
                RoomType = RoomType.ClassRoom,
                HasSmartBoard = true
            };

            // Act
            service.AddRoom(room);

            // Assert
            mockRepo.Verify(r => r.AddRoom(room), Times.Once);
        }

        [TestMethod]
        public void AddRoom_NullRoom_ShouldThrowArgumentNullException()
        {
            // Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(
                delegate { service.AddRoom(null); }
            );
            mockRepo.Verify(r => r.AddRoom(It.IsAny<Room>()), Times.Never);
        }

        //-------------------------DeleteRoom-------------------------
        [TestMethod]
        public void DeleteRoom_ValidId_ShouldCallRepositoryOnce()
        {
            // Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);
            int roomId = 1;

            // Act
            service.DeleteRoom(roomId);

            // Assert
            mockRepo.Verify(r => r.DeleteRoom(roomId), Times.Once);
        }
        [TestMethod]
        public void DeleteRoom_InvalidId_ShouldThrowArgumentException()
        {
            // Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);
            int invalidId = 0;

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(
                delegate { service.DeleteRoom(invalidId); }
            );

        }

    }
}