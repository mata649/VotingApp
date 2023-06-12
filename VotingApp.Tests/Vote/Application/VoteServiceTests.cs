using Microsoft.Extensions.Logging;
using Moq;
using VotingApp.Base.Domain;
using VotingApp.Context;
using VotingApp.Option.Domain;
using VotingApp.User.Domain;
using VotingApp.Vote.Application;
using VotingApp.Vote.Domain;
using VotingApp.Vote.Domain.DTO;
using Xunit;

namespace VotingApp.Tests.Vote.Application
{
    public class VoteServiceTests : IDisposable
    {
        private readonly VoteService _voteService;
        private readonly Mock<IOptionRepository> _optionRepositoryMock;
        private readonly Mock<IVoteRepository> _voteRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;

        private bool _disposedValue;

        public VoteServiceTests()
        {
            var logger = new Mock<ILogger<VoteService>>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            _voteRepositoryMock = new Mock<IVoteRepository>();
            _optionRepositoryMock = new Mock<IOptionRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();

            unitOfWorkMock
                .SetupGet(uow => uow.VoteRepository)
                .Returns(_voteRepositoryMock.Object);

            unitOfWorkMock
                .SetupGet(uow => uow.OptionRepository)
                .Returns(_optionRepositoryMock.Object);

            unitOfWorkMock
                .SetupGet(uow => uow.UserRepository)
                .Returns(_userRepositoryMock.Object);

            _voteService = new VoteService(unitOfWorkMock.Object, logger.Object);
        }

        [Fact]
        public void Create_UnexpectedError_ReturnsInternalError()
        {
            // Arrange
            var createDTO = new CreateVoteDTO()
            {
                OptionID = Guid.NewGuid(),
                UserID = Guid.NewGuid(),
            };
            _optionRepositoryMock
                .Setup(repo => repo.GetById(createDTO.OptionID))
                .Throws(new Exception());

            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);

            // Act
            IResponse response = _voteService.Create(createDTO);

            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void Create_OptionNotFound_ReturnsNotFoundError()
        {
            // Arrange
            var createDTO = new CreateVoteDTO()
            {
                OptionID = Guid.NewGuid(),
                UserID = Guid.NewGuid(),
            };
            _optionRepositoryMock
                .Setup(repo => repo.GetById(createDTO.OptionID))
                .Returns<OptionEntity?>(null);

            IResponse expectedResponse = new ResponseFailure("Option was not found", 404);

            // Act
            IResponse response = _voteService.Create(createDTO);

            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void Create_UserNotFound_ReturnsNotFoundError()
        {
            // Arrange
            var createDTO = new CreateVoteDTO()
            {
                OptionID = Guid.NewGuid(),
                UserID = Guid.NewGuid(),
            };
            _optionRepositoryMock
                .Setup(repo => repo.GetById(createDTO.OptionID))
                .Returns(new OptionEntity());

            _userRepositoryMock
                .Setup(repo => repo.GetById(createDTO.UserID))
                .Returns<UserEntity?>(null);

            IResponse expectedResponse = new ResponseFailure("User was not found", 404);

            // Act
            IResponse response = _voteService.Create(createDTO);

            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void Create_UserHasVoted_ReturnsForbiddenError()
        {
            // Arrange
            var userFound = new UserEntity() { ID = Guid.NewGuid() };
            var optionFound = new OptionEntity() { ID = Guid.NewGuid() };
            var createDTO = new CreateVoteDTO() { OptionID = optionFound.ID, UserID = userFound.ID };

            _optionRepositoryMock
                .Setup(repo => repo.GetById(createDTO.OptionID))
                .Returns(optionFound);

            _userRepositoryMock
                .Setup(repo => repo.GetById(createDTO.UserID))
                .Returns(userFound);

            _voteRepositoryMock
                .Setup(repo => repo.Get(It.IsAny<Filters<VoteEntity>>(), It.IsAny<Pagination>()))
                .Returns((new List<VoteEntity>() { new VoteEntity() }, 1, 1));

            IResponse expectedResponse = new ResponseFailure("You can't participate again in this poll", 403);

            // Act
            IResponse response = _voteService.Create(createDTO);

            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void Create_IsValid_ReturnsSuccessResponse()
        {
            // Arrange
            var userFound = new UserEntity() { ID = Guid.NewGuid() };
            var optionFound = new OptionEntity() { ID = Guid.NewGuid() };
            var vote = new VoteEntity()
            {
                ID = Guid.NewGuid(),
                OptionID = optionFound.ID,
                Option = optionFound,
                UserID = userFound.ID,
                User = userFound
            };
            var createDTO = new Mock<CreateVoteDTO>();

            createDTO.Setup(dto => dto.ToEntity()).Returns(vote);

            _optionRepositoryMock
                .Setup(repo => repo.GetById(vote.OptionID))
                .Returns(optionFound);

            _userRepositoryMock
                .Setup(repo => repo.GetById(vote.UserID))
                .Returns(userFound);

            _voteRepositoryMock
                .Setup(repo => repo.Get(It.IsAny<Filters<VoteEntity>>(), It.IsAny<Pagination>()))
                .Returns((new List<VoteEntity>(), 1, 1));

            IResponse expectedResponse = new ResponseSuccess(ResponseVoteDTO.FromVote(vote), 201);

            // Act
            IResponse response = _voteService.Create(createDTO.Object);

            // Assert
            Assert.Equivalent(expectedResponse, response);
            _voteRepositoryMock.Verify(repo => repo.Create(vote), Times.Once);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _voteRepositoryMock.Reset();
                    _optionRepositoryMock.Reset();
                    _userRepositoryMock.Reset();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~VoteServiceTests()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
