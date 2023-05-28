using Microsoft.Extensions.Logging;
using Moq;
using VotingApp.Base.Domain;
using VotingApp.Context;
using VotingApp.Option.Domain;
using VotingApp.Poll.Application;
using VotingApp.Poll.Domain;
using VotingApp.Poll.Domain.DTO;
using VotingApp.User.Domain;

namespace VotingApp.Tests.Poll.Application
{
    public class PollServiceTests : IDisposable
    {
        private bool disposedValue;

        public readonly PollService _pollService;

        public readonly Mock<IUserRepository> _userRepositoryMock;

        public readonly Mock<IPollRepository> _pollRepositoryMock;

        public readonly Mock<IOptionRepository> _optionRepositoryMock;

        public PollServiceTests()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _pollRepositoryMock = new Mock<IPollRepository>();
            _optionRepositoryMock = new Mock<IOptionRepository>();
            var logger = new Mock<ILogger<PollService>>();

            unitOfWorkMock
                .SetupGet(uow => uow.UserRepository)
                .Returns(_userRepositoryMock.Object);

            unitOfWorkMock
                .SetupGet(uow => uow.PollRepository)
                .Returns(_pollRepositoryMock.Object);

            unitOfWorkMock
                .SetupGet(uow => uow.OptionRepository)
                .Returns(_optionRepositoryMock.Object);

            _pollService = new PollService(unitOfWorkMock.Object, logger.Object);

        }

        [Fact]
        public void Create_UnexpectedError_ReturnsInternalError()
        {
            // Arrange
            CreatePollDTO pollDTO = new();
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);
            _userRepositoryMock.Setup(repo => repo.GetById(pollDTO.UserID))
                .Throws(new Exception());
            // Act
            IResponse response = _pollService.Create(pollDTO);

            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void Create_UserNotFound_ReturnsNotFoundError()
        {
            // Arrange
            CreatePollDTO pollDTO = new();
            IResponse expectedResponse = new ResponseFailure("User does not exist", 404);
            _userRepositoryMock.Setup(repo => repo.GetById(pollDTO.UserID))
                .Returns<UserEntity?>(null);
            // Act
            IResponse response = _pollService.Create(pollDTO);

            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void Create_WithoutOptions_ReturnsSuccessResponse()
        {
            // Arrange
            Mock<CreatePollDTO> pollDTO = new();
            PollEntity pollEntity = new()
            {
                ID = Guid.NewGuid(),
                UserID = Guid.NewGuid(),
                Question = "Is this a testing question?",
                CreatedAt = DateTime.UtcNow,
            };

            IResponse expectedResponse = new ResponseSuccess(ResponsePollDTO.FromPoll(pollEntity), 201);

            _userRepositoryMock
                .Setup(repo => repo.GetById(pollEntity.UserID))
                .Returns(new UserEntity());

            pollDTO.Setup(dto => dto.ToEntity()).Returns(pollEntity);
            // Act
            IResponse response = _pollService.Create(pollDTO.Object);

            // Assert
            Assert.Equivalent(expectedResponse, response);
            _pollRepositoryMock.Verify(repo => repo.Create(pollEntity), Times.Once);
            // Checks if the response wasn't called due to we are not adding options to the pollDTO
            _optionRepositoryMock.Verify(repo => repo.Create(It.IsAny<OptionEntity>()), Times.Never);
        }

        [Fact]
        public void Create_WithOptions_ReturnsSuccessResponse()
        {
            // Arrange
            List<PollOption> options = new()
            {
                new PollOption(),
                new PollOption(),

            };
            PollEntity pollEntity = new()
            {
                ID = Guid.NewGuid(),
                UserID = Guid.NewGuid(),
                Question = "Is this a testing question?",
                CreatedAt = DateTime.UtcNow,
            };

            Mock<CreatePollDTO> pollDTO = new();

            IResponse expectedResponse = new ResponseSuccess(ResponsePollDTO.FromPoll(pollEntity), 201);

            _userRepositoryMock.Setup(repo => repo.GetById(pollEntity.UserID))
                .Returns(new UserEntity());
            pollDTO.SetupAllProperties();
            pollDTO.Setup(dto => dto.ToEntity()).Returns(pollEntity);
            pollDTO.Object.Options = options;

            // Act
            IResponse response = _pollService.Create(pollDTO.Object);

            // Assert
            Assert.Equivalent(expectedResponse, response);
            _pollRepositoryMock.Verify(repo => repo.Create(pollEntity), Times.Once);
            // Checks if the response was called twice due to we are adding options to the pollDTO
            _optionRepositoryMock.Verify(repo => repo.Create(It.IsAny<OptionEntity>()), Times.Exactly(2));
        }

        [Fact]
        public void Update_UnexpectedError_ReturnsInternalError()
        {   // Arrange
            UpdatePollDTO pollDTO = new();
            _pollRepositoryMock.Setup(repo => repo.GetById(pollDTO.ID)).Throws(new Exception());
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);
            // Act
            IResponse response = _pollService.Update(pollDTO);
            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void Update_PollNotFound_ReturnsNotFoundError()
        {   // Arrange
            UpdatePollDTO pollDTO = new();
            _pollRepositoryMock.Setup(repo => repo.GetById(pollDTO.ID)).Returns<PollEntity?>(null);
            IResponse expectedResponse = new ResponseFailure("Poll was not found", 404);
            // Act
            IResponse response = _pollService.Update(pollDTO);
            // Assert
            Assert.Equivalent(expectedResponse, response);
        }
        [Fact]
        public void Update_PollDoesNotBelongToCurrentUser_ReturnsUnauthorizedError()
        {   // Arrange
            PollEntity pollFound = new()
            {
                ID = Guid.NewGuid(),
                UserID = Guid.NewGuid(),
            };
            UpdatePollDTO pollDTO = new() { CurrentUserID = Guid.NewGuid() };
            _pollRepositoryMock.Setup(repo => repo.GetById(pollDTO.ID)).Returns(pollFound);
            IResponse expectedResponse = new ResponseFailure("Unauthorized", 401);
            // Act
            IResponse response = _pollService.Update(pollDTO);
            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void Update_PollIsValid_ReturnsSuccessResponse()
        {   // Arrange
            PollEntity pollFound = new()
            {
                ID = Guid.NewGuid(),
                UserID = Guid.NewGuid(),
            };
            UpdatePollDTO pollDTO = new() { CurrentUserID = pollFound.UserID };
            _pollRepositoryMock.Setup(repo => repo.GetById(pollDTO.ID)).Returns(pollFound);
            IResponse expectedResponse = new ResponseSuccess(ResponsePollDTO.FromPoll(pollFound), 200);
            // Act
            IResponse response = _pollService.Update(pollDTO);
            // Assert

            Assert.Equivalent(expectedResponse, response);
            _pollRepositoryMock.Verify(repo => repo.Update(pollFound), Times.Once);
        }

        [Fact]
        public void Delete_UnexpectedError_ReturnsInternalError()
        {   // Arrange

            DeleteDTO deleteDTO = new();
            _pollRepositoryMock.Setup(repo => repo.GetById(deleteDTO.ID)).Throws(new Exception());
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);
            // Act
            IResponse response = _pollService.Delete(deleteDTO);
            // Assert
            Assert.Equivalent(expectedResponse, response);
        }


        [Fact]
        public void Delete_PollNotFound_ReturnsNotFoundError()
        {   // Arrange

            DeleteDTO deleteDTO = new();
            _pollRepositoryMock.Setup(repo => repo.GetById(deleteDTO.ID))
                .Returns<PollEntity?>(null);
            IResponse expectedResponse = new ResponseFailure("Poll was not found", 404);
            // Act
            IResponse response = _pollService.Delete(deleteDTO);
            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void Delete_PollDoesNotBelongCurrentUser_ReturnsUnauthorizedError()
        {   // Arrange

            DeleteDTO deleteDTO = new() { CurrentUserID = Guid.NewGuid() };
            PollEntity pollFound = new() { UserID = Guid.NewGuid() };
            _pollRepositoryMock.Setup(repo => repo.GetById(deleteDTO.ID))
                .Returns(pollFound);
            IResponse expectedResponse = new ResponseFailure("Unauthorized", 401);
            // Act
            IResponse response = _pollService.Delete(deleteDTO);
            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void Delete_IsValid_ReturnsResponseSuccess()
        {   // Arrange

            DeleteDTO deleteDTO = new() { CurrentUserID = Guid.NewGuid() };
            PollEntity pollFound = new() { UserID = deleteDTO.CurrentUserID };
            _pollRepositoryMock.Setup(repo => repo.GetById(deleteDTO.ID))
                .Returns(pollFound);
            IResponse expectedResponse = new ResponseSuccess(ResponsePollDTO.FromPoll(pollFound), 200);
            // Act
            IResponse response = _pollService.Delete(deleteDTO);
            // Assert
            Assert.Equivalent(expectedResponse, response);
            _pollRepositoryMock.Verify(repo => repo.Delete(pollFound.ID), Times.Once);

        }

        [Fact]
        public void Get_UnexpectedError_ReturnsInternalError()
        {
            // Arrange
            PollFiltersDTO pollDTO = new(1, 10, "", Guid.Empty);
            _pollRepositoryMock
                .Setup(repo => repo.Get(pollDTO.Filters, pollDTO.Pagination))
                .Throws(new Exception());
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);
            // Act
            IResponse response = _pollService.Get(pollDTO);
            // Assert
            Assert.Equivalent(expectedResponse, response);

        }
        [Fact]
        public void Get_WithoutData_ReturnsSuccessResponse()
        {
            // Arrange
            PollFiltersDTO pollDTO = new(1, 10, "", Guid.Empty);
            _pollRepositoryMock
                .Setup(repo => repo.Get(pollDTO.Filters, pollDTO.Pagination))
                .Returns((new List<PollEntity>(), 1, 0));

            IResponse expectedResponse = new ResponseSuccess(new Results<ResponsePollDTO>
            {
                CurrentPage = 1,
                TotalPages = 0,
                Data = Enumerable.Empty<ResponsePollDTO>()
            }, 200);
            // Act
            IResponse response = _pollService.Get(pollDTO);
            // Assert
            Assert.Equivalent(expectedResponse, response);
            _pollRepositoryMock
                .Verify(repo => repo.Get(pollDTO.Filters, pollDTO.Pagination), Times.Once);
        }
        [Fact]
        public void Get_WithtData_ReturnsSuccessResponse()
        {
            // Arrange
            PollFiltersDTO pollDTO = new(1, 10, "", Guid.Empty);
            List<PollEntity> expectedPolls = new()
            {
                new PollEntity(){ID = Guid.NewGuid(),Question = "Is this a test 1?", CreatedAt=DateTime.Now, UserID= Guid.NewGuid()},
                new PollEntity(){ID = Guid.NewGuid(),Question = "Is this a test 2?", CreatedAt=DateTime.Now, UserID= Guid.NewGuid()},
                new PollEntity(){ID = Guid.NewGuid(),Question = "Is this a test 3?", CreatedAt=DateTime.Now, UserID= Guid.NewGuid()},
            };
            _pollRepositoryMock
                .Setup(repo => repo.Get(pollDTO.Filters, pollDTO.Pagination))
                .Returns((expectedPolls, 1, 0));

            IResponse expectedResponse = new ResponseSuccess(new Results<ResponsePollDTO>
            {
                CurrentPage = 1,
                TotalPages = 0,
                Data = expectedPolls.Select(ResponsePollDTO.FromPoll)
            }, 200);
            // Act
            IResponse response = _pollService.Get(pollDTO);
            // Assert
            Assert.Equivalent(expectedResponse, response);
            _pollRepositoryMock
                .Verify(repo => repo.Get(pollDTO.Filters, pollDTO.Pagination), Times.Once);
        }
        [Fact]
        public void GetByID_UnexpectedError_ReturnsInternalError()
        {
            // Arrange
            GetByIDDTO getByIDDTO = new();
            _pollRepositoryMock
                .Setup(repo => repo.GetById(getByIDDTO.ID))
                .Throws(new Exception());
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);
            // Act
            IResponse response = _pollService.GetById(getByIDDTO);

            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void GetByID_PollNotFound_ReturnsNotFoundError()
        {
            // Arrange
            GetByIDDTO getByIDDTO = new();
            _pollRepositoryMock
                .Setup(repo => repo.GetById(getByIDDTO.ID))
                .Returns<PollEntity?>(null);

            IResponse expectedResponse = new ResponseFailure("Poll was not found", 404);

            // Act
            IResponse response = _pollService.GetById(getByIDDTO);

            // Assert
            Assert.Equivalent(expectedResponse, response);
            _pollRepositoryMock.Verify(repo => repo.GetById(getByIDDTO.ID), Times.Once);
        }


        [Fact]
        public void GetByID_PollFound_ReturnsSuccessResponse()
        {
            // Arrange
            PollEntity pollFound = new PollEntity();
            GetByIDDTO getByIDDTO = new();
            _pollRepositoryMock
                .Setup(repo => repo.GetById(getByIDDTO.ID))
                .Returns(pollFound);

            IResponse expectedResponse = new ResponseSuccess(ResponsePollDTO.FromPoll(pollFound), 200);

            // Act
            IResponse response = _pollService.GetById(getByIDDTO);

            // Assert
            Assert.Equivalent(expectedResponse, response);
            _pollRepositoryMock.Verify(repo => repo.GetById(getByIDDTO.ID), Times.Once);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _pollRepositoryMock.Reset();
                    _userRepositoryMock.Reset();
                    _optionRepositoryMock.Reset();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PollServiceTests()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
