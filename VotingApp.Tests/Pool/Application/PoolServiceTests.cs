using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using VotingApp.Base.Domain;
using VotingApp.Context;
using VotingApp.Pool.Application;
using VotingApp.Pool.Domain;
using VotingApp.Pool.Domain.DTO;
using VotingApp.User.Domain;

namespace VotingApp.Tests.Pool.Application
{
    public class PoolServiceTests : IDisposable
    {
        private bool disposedValue;

        public readonly PoolService _poolServiceMock;

        public readonly Mock<IUserRepository> _userRepositoryMock;

        public readonly Mock<IPoolRepository> _poolRepositoryMock;

        public PoolServiceTests()
        {
            var votingAppContextMock = new Mock<VotingAppContext>();
            var unitOfWorkMock = new Mock<UnitOfWork>(votingAppContextMock.Object);
            _userRepositoryMock = new Mock<IUserRepository>();
            _poolRepositoryMock = new Mock<IPoolRepository>();

            var logger = new Mock<ILogger<PoolService>>();

            unitOfWorkMock
                .SetupGet(uow => uow.UserRepository)
                .Returns(_userRepositoryMock.Object);

            unitOfWorkMock .SetupGet(uow => uow.PoolRepository)
                .Returns(_poolRepositoryMock.Object);

            _poolServiceMock = new PoolService(unitOfWorkMock.Object, logger.Object);

        }

        [Fact]
        public void Create_UnexpectedError_ReturnsInternalError()
        {
            // Arrange
            CreatePoolDTO poolDTO = new();
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);
            _userRepositoryMock.Setup(urm=>urm.GetById(poolDTO.UserID))
                .Throws(new Exception());
            // Act
            IResponse response = _poolServiceMock.Create(poolDTO);
            
            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _poolRepositoryMock.Reset();
                    _userRepositoryMock.Reset();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PoolServiceTests()
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
