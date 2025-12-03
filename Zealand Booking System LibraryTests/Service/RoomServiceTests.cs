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
        public void AddRoom_ShouldCallRepositoryAddRoom_Once()
        {
            // Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomName = "TestLokale",
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
        {// Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            Room room = new Room
            {
                RoomName = null, // invalid
                RoomLocation = "A123",
                RoomDescription = "Test",
                RoomType = RoomType.ClassRoom,
                HasSmartBoard = true
            };

            // Act + Assert
            Assert.ThrowsException<ArgumentException>(() => service.AddRoom(room));
        }

        //-------------------------DeleteRoom-------------------------
        [TestMethod]
        public void DeleteRoom_ValidId_ShouldCallRepositoryOnce()
        {
            // Arrange:
            // Vi opretter et mock repository og RoomService,
            // og bruger et gyldigt ID (1)
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);
            int roomId = 1;

            // Act:
            // Metoden skal køre uden fejl
            service.DeleteRoom(roomId);

            // Assert:
            // Repositoryets DeleteRoom() SKAL blive kaldt præcis én gang
            mockRepo.Verify(r => r.DeleteRoom(roomId), Times.Once);
        }

        [TestMethod]
        public void DeleteRoom_InvalidId_Zero_ShouldThrowArgumentException()
        {
            // Arrange:
            // Bruger ID = 0 (ulovligt, da id <= 0 skal give exception)
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            // Act & Assert:
            // Vi forventer at metoden kaster en ArgumentException
            Assert.ThrowsException<ArgumentException>(() => service.DeleteRoom(0));
        }

        [TestMethod]
        public void DeleteRoom_InvalidId_Negative_ShouldThrowArgumentException()
        {
            // Arrange:
            // Bruger et negativt ID (ulovligt)
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            // Act & Assert:
            // Metoden skal kaste en ArgumentException
            Assert.ThrowsException<ArgumentException>(() => service.DeleteRoom(-5));
        }

        [TestMethod]
        public void DeleteRoom_InvalidId_MinValue_ShouldThrowArgumentException()
        {
            // Arrange:
            // Bruger det mindst mulige int (ekstrem edge case)
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            // Act & Assert:
            // Metoden skal stadig kaste exception
            Assert.ThrowsException<ArgumentException>(() => service.DeleteRoom(int.MinValue));
        }

        [TestMethod]
        public void DeleteRoom_Valid_LargeId_ShouldCallRepository()
        {
            // Arrange:
            // Bruger et stort gyldigt ID (int.MaxValue)
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);
            int roomId = int.MaxValue;

            // Act:
            // Skal gennemføres uden fejl
            service.DeleteRoom(roomId);

            // Assert:
            // Repositoryets DeleteRoom() SKAL kaldes én gang
            mockRepo.Verify(r => r.DeleteRoom(roomId), Times.Once);
        }
        //-----------------------------UpdateRoom-----------------
        [TestMethod]
        public void UpdateRoom_ValidRoom_ShouldCallRepositoryOnce()
        {
            // Arrange
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = 1,
                RoomName = "Test",
                RoomLocation = "A100",
                RoomDescription = "Desc",
                RoomType = RoomType.ClassRoom,
                HasSmartBoard = true
            };

            // Act
            service.UpdateRoom(room);

            // Assert
            mockRepo.Verify(r => r.UpdateRoom(room), Times.Once);
        }

        //validering
        [TestMethod]
        public void UpdateRoom_NullRoom_ShouldThrowArgumentNullException()
        {
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                service.UpdateRoom(null);
            });
        }

        [TestMethod]
        public void UpdateRoom_InvalidRoomId_ShouldThrowArgumentException()
        {
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = 0,
                RoomName = "Test",
                RoomLocation = "A100",
                RoomType = RoomType.ClassRoom
            };

            Assert.ThrowsException<ArgumentException>(() =>
            {
                service.UpdateRoom(room);
            });
        }

        [TestMethod]
        public void UpdateRoom_EmptyRoomName_ShouldThrowArgumentException()
        {
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = 1,
                RoomName = "",
                RoomLocation = "A100",
                RoomType = RoomType.ClassRoom
            };

            Assert.ThrowsException<ArgumentException>(() =>
            {
                service.UpdateRoom(room);
            });
        }

        [TestMethod]
        public void UpdateRoom_NullRoomName_ShouldThrowArgumentException()
        {
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = 1,
                RoomName = null,
                RoomLocation = "A100",
                RoomType = RoomType.ClassRoom
            };

            Assert.ThrowsException<ArgumentException>(() =>
            {
                service.UpdateRoom(room);
            });
        }

        [TestMethod]
        public void UpdateRoom_EmptyRoomLocation_ShouldThrowArgumentException()
        {
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = 1,
                RoomName = "Test",
                RoomLocation = "",
                RoomType = RoomType.ClassRoom
            };

            Assert.ThrowsException<ArgumentException>(() =>
            {
                service.UpdateRoom(room);
            });
        }

        [TestMethod]
        public void UpdateRoom_NullRoomLocation_ShouldThrowArgumentException()
        {
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = 1,
                RoomName = "Test",
                RoomLocation = null,
                RoomType = RoomType.ClassRoom
            };

            Assert.ThrowsException<ArgumentException>(() =>
            {
                service.UpdateRoom(room);
            });
        }

        [TestMethod]
        public void UpdateRoom_InvalidRoomType_ShouldThrowArgumentException()
        {
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = 1,
                RoomName = "Test",
                RoomLocation = "A100",
                RoomType = (RoomType)999 // invalid enum value
            };

            Assert.ThrowsException<ArgumentException>(() =>
            {
                service.UpdateRoom(room);
            });
        }

        // Ensure that repository is not called when validation fails
        [TestMethod]
        public void UpdateRoom_InvalidRoom_ShouldNotCallRepository()
        {
            var mockRepo = new Mock<IRoomRepository>();
            var service = new RoomService(mockRepo.Object);

            var room = new Room
            {
                RoomID = -1,                 // invalid
                RoomName = "Test",
                RoomLocation = "A100",
                RoomType = RoomType.ClassRoom
            };

            try { service.UpdateRoom(room); }
            catch { /* ignore exception */ }

            mockRepo.Verify(r => r.UpdateRoom(It.IsAny<Room>()), Times.Never);
        }
    }
}