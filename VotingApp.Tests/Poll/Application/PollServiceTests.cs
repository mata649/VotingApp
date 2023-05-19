using Microsoft.Extensions.Logging;
using Moq;
using VotingApp.Base.Domain;
using VotingApp.Context;
using VotingApp.Poll.Application;
using VotingApp.Poll.Domain;
using VotingApp.Poll.Domain.DTO;
using VotingApp.User.Domain;

namespace VotingApp.Tests.Poll.Application
{
    public class PollServiceTests : IDisposable
    {
        private bool disposedValue;

        public readonly PollService _pollServiceMock;

        public readonly Mock<IUserRepository> _userRepositoryMock;

        public readonly Mock<IPollRepository> _pollRepositoryMock;

        public PollServiceTests()
        {
            var votingAppContextMock = new Mock<VotingAppContext>();
            var unitOfWorkMock = new Mock<UnitOfWork>(votingAppContextMock.Object);
            _userRepositoryMock = new Mock<IUserRepository>();
            _pollRepositoryMock = new Mock<IPollRepository>();

            var logger = new Mock<ILogger<PollService>>();

            unitOfWorkMock
                .SetupGet(uow => uow.UserRepository)
                .Returns(_userRepositoryMock.Object);

            unitOfWorkMock.SetupGet(uow => uow.PollRepository)
                .Returns(_pollRepositoryMock.Object);

            _pollServiceMock = new PollService(unitOfWorkMock.Object, logger.Object);

        }

        [Fact]
        public void Create_UnexpectedError_ReturnsInternalError()
        {
            // Arrange
            CreatePollDTO pollDTO = new();
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);
            _userRepositoryMock.Setup(urm => urm.GetById(pollDTO.UserID))
                .Throws(new Exception());
            // Act
            IResponse response = _pollServiceMock.Create(pollDTO);

            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void Create_UserNotFound_ReturnsNotFoundError()
        {
            // Arrange
            CreatePollDTO pollDTO = new();
            IResponse expectedResponse = new ResponseFailure("User does not exist", 404);
            _userRepositoryMock.Setup(urm => urm.GetById(pollDTO.UserID))
                .Returns<UserEntity?>(null);
            // Act
            IResponse response = _pollServiceMock.Create(pollDTO);

            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _pollRepositoryMock.Reset();
                    _userRepositoryMock.Reset();
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
