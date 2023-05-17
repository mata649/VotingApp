

using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Cryptography;
using System.Text;
using VotingApp.Base.Domain;
using VotingApp.Context;
using VotingApp.User.Application;
using VotingApp.User.Domain;
using VotingApp.User.Domain.DTO;

namespace VotingApp.Tests.User.Application
{
    public class UserServiceTests : IDisposable
    {
        private readonly IUserService _userServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private bool _disposedValue;

        public UserServiceTests()
        {
            var logger = new Mock<ILogger<UserService>>();
            var votingAppContext = new Mock<VotingAppContext>();

            var unitOfWorkMock = new Mock<UnitOfWork>(votingAppContext.Object);

            _userRepositoryMock = new Mock<IUserRepository>();
            unitOfWorkMock
                .SetupGet(uow => uow.UserRepository)
                .Returns(_userRepositoryMock.Object);
            _userServiceMock = new UserService(unitOfWorkMock.Object, logger.Object);
        }
        private string HashPassword(string password)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        [Fact]
        public void Login_UnexpectedError_ReturnsInternalError()
        {   // Arrange
            LoginUserDTO loginUserDTO = new();
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);
            _userRepositoryMock.Setup(ur => ur.GetByEmail(loginUserDTO.Email)).Throws(new Exception());
            // Act
            IResponse resp = _userServiceMock.Login(loginUserDTO);
            // Assert
            Assert.Equivalent(expectedResponse, resp);
        }

        [Fact]
        public void Login_UserNotFound_ReturnsInvalidCredentialsError()
        {   // Arrange
            LoginUserDTO loginUserDTO = new() { Email = "john@doe.com" };
            IResponse expectedResponse = new ResponseFailure("Invalid credentials", 401);
            _userRepositoryMock.Setup(ur => ur.GetByEmail(loginUserDTO.Email))
                .Returns<UserEntity?>(null);
            // Act
            IResponse resp = _userServiceMock.Login(loginUserDTO);
            // Assert
            Assert.Equivalent(expectedResponse, resp);
        }

        [Fact]
        public void Login_InvalidPassword_ReturnsInvalidCredentialsError()
        {
            // Arrange
            LoginUserDTO loginUserDTO = new() { Email = "john@doe.com", Password = "Testing1234" };
            UserEntity userFound = new()
            {
                ID = Guid.NewGuid(),
                Email = loginUserDTO.Email,
                Name = "John Doe",
                Password = "fdd8e762939f2e2c81d22501"
            };
            IResponse expectedResponse = new ResponseFailure("Invalid credentials", 401);
            _userRepositoryMock.Setup(ur => ur.GetByEmail(loginUserDTO.Email))
                .Returns(userFound);
            // Act
            IResponse resp = _userServiceMock.Login(loginUserDTO);
            // Assert
            Assert.Equivalent(expectedResponse, resp);
        }

        [Fact]
        public void Login_Valid_ReturnsResponseSuccess()
        {
            // Arrange
            LoginUserDTO loginUserDTO = new()
            {
                Email = "john@doe.com",
                Password = "Testing1234"
            };
            UserEntity userFound = new()
            {
                ID = Guid.NewGuid(),
                Email = loginUserDTO.Email,
                Name = "John Doe",
                Password = HashPassword("Testing1234")
            };
            IResponse expectedResponse = new ResponseSuccess(ResponseUserDTO.FromUser(userFound), 200);
            _userRepositoryMock.Setup(ur => ur.GetByEmail(loginUserDTO.Email))
                .Returns(userFound);
            // Act
            IResponse resp = _userServiceMock.Login(loginUserDTO);
            // Assert
            Assert.Equivalent(expectedResponse, resp);
        }
        [Fact]
        public void ChangePassword_UnexpectedError_ReturnsInternalError()
        {   // Arrange
            ChangePasswordDTO changePasswordDTO = new()
            {
                ID = Guid.NewGuid(),
                CurrentUserID = Guid.NewGuid(),
                NewPassword = "NewPassword",
                OldPassword = "OldPassword"
            };
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);
            _userRepositoryMock.Setup(urm => urm.GetById(changePasswordDTO.ID)).Throws(new Exception());
            // Act
            IResponse resp = _userServiceMock.ChangePassword(changePasswordDTO);
            // Assert
            Assert.Equivalent(resp, expectedResponse);
        }

        [Fact]
        public void ChangePassword_UserNotFound_ReturnsNotFoundError()
        {   // Arrange
            ChangePasswordDTO changePasswordDTO = new()
            {
                ID = Guid.NewGuid(),
                CurrentUserID = Guid.NewGuid(),
                NewPassword = "NewPassword",
                OldPassword = "OldPassword"
            };
            IResponse expectedResponse = new ResponseFailure("User was not found", 404);
            _userRepositoryMock
                .Setup(urm => urm.GetById(changePasswordDTO.ID))
                .Returns<UserEntity?>(null);
            // Act
            IResponse resp = _userServiceMock.ChangePassword(changePasswordDTO);
            // Assert
            Assert.Equivalent(resp, expectedResponse);
        }

        [Fact]
        public void ChangePassword_CurrentUserHasDifferentID_ReturnsUnauthorizedError()
        {   // Arrange
            UserEntity userFound = new()
            {
                ID = Guid.NewGuid(),

            };
            ChangePasswordDTO changePasswordDTO = new()
            {
                ID = userFound.ID,
                CurrentUserID = Guid.NewGuid(),
                NewPassword = "NewPassword",
                OldPassword = "OldPassword"
            };
            IResponse expectedResponse = new ResponseFailure("Unauthorized", 401);
            _userRepositoryMock
                .Setup(urm => urm.GetById(changePasswordDTO.ID))
                .Returns(userFound);
            // Act
            IResponse resp = _userServiceMock.ChangePassword(changePasswordDTO);
            // Assert
            Assert.Equivalent(resp, expectedResponse);
        }
        [Fact]
        public void ChangePassword_OldPasswordIsIncorrect_ReturnsPasswordIncorrectError()
        {   // Arrange
            UserEntity userFound = new()
            {
                ID = Guid.NewGuid(),
                Password = HashPassword("IncorrectOldPassword")
            };
            ChangePasswordDTO changePasswordDTO = new()
            {
                ID = userFound.ID,
                CurrentUserID = userFound.ID,
                NewPassword = "NewPassword",
                OldPassword = "OldPassword"
            };
            IResponse expectedResponse = new ResponseFailure("Password incorrect", 401);
            _userRepositoryMock
                .Setup(urm => urm.GetById(changePasswordDTO.ID))
                .Returns(userFound);
            // Act
            IResponse resp = _userServiceMock.ChangePassword(changePasswordDTO);
            // Assert
            Assert.Equivalent(resp, expectedResponse);
        }
        [Fact]
        public void ChangePassword_ValidInformation_ReturnsSuccessResponse()
        {   // Arrange
            UserEntity userFound = new()
            {
                ID = Guid.NewGuid(),
                Password = HashPassword("OldPassword")
            };
            ChangePasswordDTO changePasswordDTO = new()
            {
                ID = userFound.ID,
                CurrentUserID = userFound.ID,
                NewPassword = "NewPassword",
                OldPassword = "OldPassword"
            };
            IResponse expectedResponse = new ResponseSuccess(ResponseUserDTO.FromUser(userFound), 200);
            _userRepositoryMock
                .Setup(urm => urm.GetById(changePasswordDTO.ID))
                .Returns(userFound);
            // Act
            IResponse resp = _userServiceMock.ChangePassword(changePasswordDTO);
            // Assert
            Assert.Equivalent(resp, expectedResponse);
        }
        [Fact]
        public void Create_UnexpectedError_ReturnsInternalError()
        {
            //Arrange
            CreateUserDTO createUserDTO = new()
            {
                Email = "john@doe.com",
                Name = "John Doe",
                Password = "JohnDoe1234"
            };
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);
            _userRepositoryMock.Setup(ur => ur.GetByEmail(createUserDTO.Email))
                .Throws(new Exception());

            // Act
            var response = _userServiceMock.Create(createUserDTO);

            // Assert
            Assert.Equivalent(expectedResponse, response);

        }

        [Fact]
        public void Create_UserWithSameEmail_ReturnsConflictError()
        {
            //Arrange
            CreateUserDTO createUserDTO = new()
            {
                Email = "john@doe.com",
                Name = "John Doe",
                Password = "JohnDoe1234"
            };
            IResponse expectedResponse = new ResponseFailure("The email is already registered", 409);
            _userRepositoryMock.Setup(ur => ur.GetByEmail(createUserDTO.Email))
                .Returns(new UserEntity()
                {
                    ID = Guid.NewGuid(),
                    Email = createUserDTO.Email,
                    Name = createUserDTO.Name,
                });

            // Act
            var response = _userServiceMock.Create(createUserDTO);

            // Assert
            Assert.Equivalent(expectedResponse, response);

        }

        [Fact]
        public void Create_ValidUser_ReturnsResponseSucces()
        {
            //Arrange
            // I have to use a mock due to I have to know what's going to be the ID 
            // of the user. This value is generated in the ToEntity function.
            var createUserDTO = new Mock<CreateUserDTO>();
            UserEntity userToCreate = new()
            {
                ID = Guid.NewGuid(),
                Email = "john@doe.com",
                Name = "John Doe",
                Password = "John1234",
            };
            IResponse expectedResponse = new ResponseSuccess(ResponseUserDTO.FromUser(userToCreate), 201);

            _userRepositoryMock.Setup(ur => ur.GetByEmail(userToCreate.Email))
                               .Returns<UserEntity?>(null);

            createUserDTO.Setup(dto => dto.ToEntity()).Returns(userToCreate);

            // Act
            var response = _userServiceMock.Create(createUserDTO.Object);

            // Assert
            Assert.Equivalent(expectedResponse, response);

        }

        [Fact]
        public void Update_UnexpectedError_ReturnsInternalError()
        {
            //Arrange
            var updateUserDTO = new Mock<UpdateUserDTO>();
            UserEntity userToUpdate = new()
            {
                ID = Guid.NewGuid(),
                Email = "john@doe.com",
                Name = "John Doe",
                Password = "John1234",
            };
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);

            updateUserDTO.Setup(dto => dto.ToEntity())
                .Returns(userToUpdate);
            _userRepositoryMock.Setup(ur => ur.GetById(userToUpdate.ID))
                .Throws(new Exception());


            // Act
            var response = _userServiceMock.Update(updateUserDTO.Object);

            // Assert
            Assert.Equivalent(expectedResponse, response);

        }

        [Fact]
        public void Update_UserNotFound_ReturnsNotFoundError()
        {
            //Arrange
            var updateUserDTO = new Mock<UpdateUserDTO>();
            UserEntity userToUpdate = new()
            {
                ID = Guid.NewGuid(),
                Email = "john@doe.com",
                Name = "John Doe",
                Password = "John1234",
            };
            IResponse expectedResponse = new ResponseFailure("User was not found", 404);

            updateUserDTO.Setup(dto => dto.ToEntity())
                .Returns(userToUpdate);

            _userRepositoryMock.Setup(ur => ur.GetById(userToUpdate.ID))
                .Returns<UserEntity?>(null);


            // Act
            var response = _userServiceMock.Update(updateUserDTO.Object);

            // Assert
            Assert.Equivalent(expectedResponse, response);

        }

        [Fact]
        public void Update_CurrentUserHasDifferentID_ReturnsUnauthorizedError()
        {
            //Arrange
            var updateUserDTO = new Mock<UpdateUserDTO>();
            updateUserDTO.Object.CurrentUserID = Guid.NewGuid();
            UserEntity userToUpdate = new()
            {
                ID = Guid.NewGuid(),
                Email = "john@doe.com",
                Name = "John Doe",
                Password = "John1234",
            };
            UserEntity userFound = new() { ID = Guid.NewGuid() };
            IResponse expectedResponse = new ResponseFailure("Unauthorized", 401);

            updateUserDTO.Setup(dto => dto.ToEntity())
                .Returns(userToUpdate);

            _userRepositoryMock.Setup(ur => ur.GetById(userToUpdate.ID))
                .Returns(userFound);


            // Act
            var response = _userServiceMock.Update(updateUserDTO.Object);

            // Assert
            Assert.Equivalent(expectedResponse, response);

        }

        [Fact]
        public void Update_ValidUser_ReturnsSuccessResponse()
        {
            //Arrange
            var updateUserDTO = new Mock<UpdateUserDTO>();

            UserEntity userToUpdate = new()
            {
                ID = Guid.NewGuid(),
                Email = "john@doe.com",
                Name = "John Doe",
                Password = "John1234",
            };

            updateUserDTO.Object.CurrentUserID = userToUpdate.ID;
            UserEntity userFound = new() { ID = userToUpdate.ID };
            IResponse expectedResponse = new ResponseSuccess(ResponseUserDTO.FromUser(userToUpdate), 200);

            updateUserDTO.Setup(dto => dto.ToEntity())
                .Returns(userToUpdate);

            _userRepositoryMock.Setup(ur => ur.GetById(userToUpdate.ID))
                .Returns(userFound);


            // Act
            var response = _userServiceMock.Update(updateUserDTO.Object);

            // Assert
            Assert.Equivalent(expectedResponse, response);

        }

        [Fact]
        public void Get_UnexpectedError_ReturnsInternalError()
        {
            // Arrange
            UserFiltersDTO userFiltersDTO = new(1, 10, "", "");
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);
            _userRepositoryMock.Setup(urm => urm.Get(userFiltersDTO.Filters, userFiltersDTO.Pagination))
                .Throws(new Exception());
            // Act
            IResponse resp = _userServiceMock.Get(userFiltersDTO);

            // Assert
            Assert.Equivalent(expectedResponse, resp);
        }

        [Fact]
        public void Get_WithoutUsers_ReturnsResponseSuccessWithEmptyData()
        {
            // Arrange
            UserFiltersDTO userFiltersDTO = new(1, 10, "", "");
            IResponse expectedResponse = new ResponseSuccess(new Results<ResponseUserDTO>()
            {
                CurrentPage = 1,
                TotalPages = 0,
                Data = Enumerable.Empty<ResponseUserDTO>()
            }, 200);

            _userRepositoryMock.Setup(urm => urm.Get(userFiltersDTO.Filters, userFiltersDTO.Pagination))
                .Returns((new List<UserEntity>(), 1, 0));
            // Act
            IResponse resp = _userServiceMock.Get(userFiltersDTO);

            // Assert
            Assert.Equivalent(expectedResponse, resp);
        }
        [Fact]
        public void Get_Withsers_ReturnsResponseSuccessWithData()
        {
            // Arrange
            UserFiltersDTO userFiltersDTO = new(1, 10, "", "");
            List<UserEntity> expectedUsers = new() {
                new UserEntity() { ID = Guid.NewGuid(), Email = "jonh1@doe.com", Name = "John Doe 1" },
                new UserEntity() { ID = Guid.NewGuid(), Email = "jonh2@doe.com", Name = "John Doe 2" },
                new UserEntity() { ID = Guid.NewGuid(), Email = "jonh3@doe.com", Name = "John Doe 3" },
            };

            IResponse expectedResponse = new ResponseSuccess(new Results<ResponseUserDTO>()
            {
                CurrentPage = 1,
                TotalPages = 10,
                Data = expectedUsers.Select(ResponseUserDTO.FromUser)
            }, 200);

            _userRepositoryMock.Setup(urm => urm.Get(userFiltersDTO.Filters, userFiltersDTO.Pagination))
                .Returns((expectedUsers, 1, 10));
            // Act
            IResponse resp = _userServiceMock.Get(userFiltersDTO);

            // Assert
            Assert.Equivalent(expectedResponse, resp);
        }

        [Fact]
        public void Delete_UnexpectedError_ReturnsInternalError()
        {
            // Arrange
            DeleteDTO deleteDTO = new() { ID = Guid.NewGuid(), CurrentUserID = Guid.NewGuid() };
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);
            _userRepositoryMock.Setup(urm => urm.GetById(deleteDTO.ID)).Throws(new Exception());
            // Act
            IResponse resp = _userServiceMock.Delete(deleteDTO);
            // Assert
            Assert.Equivalent(expectedResponse, resp);

        }
        [Fact]
        public void Delete_UserNotFound_ReturnsNotFoundError()
        {
            // Arrange
            DeleteDTO deleteDTO = new() { ID = Guid.NewGuid(), CurrentUserID = Guid.NewGuid() };
            IResponse expectedResponse = new ResponseFailure("User was not found", 404);
            _userRepositoryMock.Setup(urm => urm.GetById(deleteDTO.ID)).Returns<UserEntity?>(null);
            // Act
            IResponse resp = _userServiceMock.Delete(deleteDTO);
            // Assert
            Assert.Equivalent(expectedResponse, resp);

        }
        [Fact]
        public void Delete_CurrentUserIDIsDifferent_ReturnsUnauthorizedError()
        {
            // Arrange
            UserEntity user = new() { ID = Guid.NewGuid() };
            DeleteDTO deleteDTO = new() { ID = user.ID, CurrentUserID = Guid.NewGuid() };
            IResponse expectedResponse = new ResponseFailure("Unauthorized", 401);
            _userRepositoryMock.Setup(urm => urm.GetById(deleteDTO.ID)).Returns(user);
            // Act
            IResponse resp = _userServiceMock.Delete(deleteDTO);
            // Assert
            Assert.Equivalent(expectedResponse, resp);

        }

        [Fact]
        public void Delete_ValidRequest_ReturnsSuccessResponse()
        {
            // Arrange
            UserEntity user = new() { ID = Guid.NewGuid() };
            DeleteDTO deleteDTO = new() { ID = user.ID, CurrentUserID = user.ID };
            IResponse expectedResponse = new ResponseSuccess(ResponseUserDTO.FromUser(user), 200);
            _userRepositoryMock.Setup(urm => urm.GetById(deleteDTO.ID)).Returns(user);
            // Act
            IResponse resp = _userServiceMock.Delete(deleteDTO);
            // Assert
            Assert.Equivalent(expectedResponse, resp);

        }

        [Fact]
        public void GetByID_UnexpectedError_ReturnsInternalError()
        {
            // Arrange
            GetByIDDTO getByIDDTO = new() { ID = Guid.NewGuid() };
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);
            _userRepositoryMock.Setup(urm => urm.GetById(getByIDDTO.ID)).Throws(new Exception());
            // Act
            IResponse resp = _userServiceMock.GetById(getByIDDTO);
            // Assert
            Assert.Equivalent(expectedResponse, resp);
        }
        [Fact]
        public void GetByID_UserNotFound_ReturnsNotFoundError()
        {
            // Arrange
            GetByIDDTO getByIDDTO = new() { ID = Guid.NewGuid() };
            IResponse expectedResponse = new ResponseFailure("User was not found", 404);
            _userRepositoryMock.Setup(urm => urm.GetById(getByIDDTO.ID)).Returns<UserEntity?>(null);
            // Act
            IResponse resp = _userServiceMock.GetById(getByIDDTO);
            // Assert
            Assert.Equivalent(expectedResponse, resp);

        }

        [Fact]
        public void GetByID_UserFound_ReturnsSuccessResponse()
        {
            // Arrange
            GetByIDDTO getByIDDTO = new() { ID = Guid.NewGuid() };
            UserEntity userFound = new()
            {
                ID = Guid.NewGuid(),
                Name = "John Doe",
                Email = "john@doe.com",
                Password = "testing"
            };
            IResponse expectedResponse = new ResponseSuccess(ResponseUserDTO.FromUser(userFound), 200);
            _userRepositoryMock.Setup(urm => urm.GetById(getByIDDTO.ID)).Returns(userFound);
            // Act
            IResponse resp = _userServiceMock.GetById(getByIDDTO);
            // Assert
            Assert.Equivalent(expectedResponse, resp);

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _userRepositoryMock.Reset();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }


        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
