using Moq;
using Xunit;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System_Tests
{
    public class UserServiceTests
    {
        [Fact]
        public void Login_ReturnsUser_WhenCredentialsAreValid()
        {
            // Arrange
            Mock<IUserRepository> repoMock = new Mock<IUserRepository>();

            string username = "simon";
            string plainPassword = "password123";
            string hashedPassword = PasswordHasher.Hash(plainPassword);

            Account storedAccount = new Account();
            storedAccount.AccountID = 1;
            storedAccount.Username = username;
            storedAccount.PasswordHash = hashedPassword;
            storedAccount.Role = "Student";

            repoMock
                .Setup(r => r.GetByUsername(username))
                .Returns(storedAccount);

            UserService service = new UserService(repoMock.Object);

            // Act
            Account result = service.Login(username, plainPassword);

            // Assert (happy path)
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(storedAccount.AccountID, result.AccountID);
            Xunit.Assert.Equal(storedAccount.Username, result.Username);
        }

        [Fact]
        public void Login_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            Mock<IUserRepository> repoMock = new Mock<IUserRepository>();

            string username = "ukendt";
            string password = "whatever";

            repoMock
                .Setup(r => r.GetByUsername(username))
                .Returns((Account)null);

            UserService service = new UserService(repoMock.Object);

            // Act
            Account result = service.Login(username, password);

            // Assert (edge case)
            Xunit.Assert.Null(result);
        }

        [Fact]
        public void Login_ReturnsNull_WhenPasswordIsWrong()
        {
            // Arrange
            Mock<IUserRepository> repoMock = new Mock<IUserRepository>();

            string username = "simon";
            string correctPassword = "correct";
            string wrongPassword = "wrong";

            string hashedCorrect = PasswordHasher.Hash(correctPassword);

            Account storedAccount = new Account();
            storedAccount.AccountID = 2;
            storedAccount.Username = username;
            storedAccount.PasswordHash = hashedCorrect;
            storedAccount.Role = "Student";

            repoMock
                .Setup(r => r.GetByUsername(username))
                .Returns(storedAccount);

            UserService service = new UserService(repoMock.Object);

            // Act
            Account result = service.Login(username, wrongPassword);

            // Assert (edge case)
            Xunit.Assert.Null(result);
        }

        [Fact]
        public void Create_HashesPassword_AndCallsRepositoryWithHashedPassword()
        {
            // Arrange
            Mock<IUserRepository> repoMock = new Mock<IUserRepository>();

            Account newUser = new Account();
            newUser.Username = "nyBruger";
            newUser.PasswordHash = "plainPassword";

            string role = "Student";

            UserService service = new UserService(repoMock.Object);

            // Act
            service.Create(newUser, role);

            // Assert (happy path + tjek hashing)
            repoMock.Verify(
                r => r.CreateUser(
                    It.Is<Account>(
                        a =>
                            a.Username == "nyBruger" &&
                            a.PasswordHash != "plainPassword" &&
                            PasswordHasher.Verify(a.PasswordHash, "plainPassword")
                    ),
                    role
                ),
                Times.Once
            );
        }

        [Fact]
        public void DeleteUser_CallsRepositoryWithSameId()
        {
            // Arrange
            Mock<IUserRepository> repoMock = new Mock<IUserRepository>();
            UserService service = new UserService(repoMock.Object);
            int id = 10;

            // Act
            service.DeleteUser(id);

            // Assert (simple happy path)
            repoMock.Verify(r => r.DeleteUser(id), Times.Once);
        }
    }
}
