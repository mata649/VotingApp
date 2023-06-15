using Microsoft.Extensions.Logging;
using Moq;
using VotingApp.Base.Domain;
using VotingApp.Context;
using VotingApp.Option.Domain;
using VotingApp.Vote.Application;
using VotingApp.Vote.Domain;
using VotingApp.Vote.Domain.DTO;

namespace VotingApp.Tests.Vote.Application
{
    public class VoteNotificationServiceTests
    {
        private readonly Mock<IVoteCountCache> _voteCountCacheMock;
        private readonly Mock<IVoteRepository> _voteRepositoryMock;
        private readonly VoteNotificationService _voteNotificationService;

        public VoteNotificationServiceTests()
        {
            _voteCountCacheMock = new Mock<IVoteCountCache>();
            _voteRepositoryMock = new Mock<IVoteRepository>();

            var logger = new Mock<ILogger<VoteNotificationService>>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork
                .SetupGet(unit => unit.VoteRepository)
                .Returns(_voteRepositoryMock.Object);

            _voteNotificationService = new VoteNotificationService(
                _voteCountCacheMock.Object,
                unitOfWork.Object,
                logger.Object);

        }

        [Fact]
        public void AddVoteToDashboard_UnexpectedError_ReturnsInternalError()
        {
            // Arrange
            var option = new OptionEntity() { ID = Guid.NewGuid(), PollID = Guid.NewGuid() };
            _voteCountCacheMock
                .Setup(cache => cache.GetCount(option.PollID))
                .Throws(new Exception());
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);

            // Act
            IResponse response = _voteNotificationService.AddVoteToDashboard(option);

            // Assert
            Assert.Equivalent(expectedResponse, response);

        }

        [Fact]
        public void AddVoteToDashboard_CacheGetCountIsNullAndRepoGetByPollIsNull_ReturnsNotFoundError()
        {
            // Arrange
            var option = new OptionEntity() { ID = Guid.NewGuid() };
            _voteCountCacheMock
                .Setup(cache => cache.GetCount(option.ID))
                .Returns<CountByOptionDTO?>(null);

            _voteRepositoryMock
                .Setup(repo => repo.VotesByPoll(option.PollID))
                .Returns<CountByOptionDTO?>(null);
            IResponse expectedResponse = new ResponseFailure("Count was not found", 404);

            // Act
            IResponse response = _voteNotificationService.AddVoteToDashboard(option);

            // Assert
            Assert.Equivalent(expectedResponse, response);

        }

        [Fact]
        public void AddVoteToDashboard_CacheGetCountIsNullAndRepoCountNotFound_ReturnsNotFoundError()
        {
            // Arrange
            var option = new OptionEntity() { ID = Guid.NewGuid() };
            _voteCountCacheMock
                .Setup(cache => cache.GetCount(option.ID))
                .Returns<CountByOptionDTO?>(null);
            _voteRepositoryMock
                .Setup(repo => repo.VotesByPoll(option.PollID))
                .Returns(new List<CountByOptionDTO>());
            IResponse expectedResponse = new ResponseFailure("Count was not found", 404);

            // Act
            IResponse response = _voteNotificationService.AddVoteToDashboard(option);

            // Assert
            Assert.Equivalent(expectedResponse, response);

        }
        [Fact]
        public void AddVoteToDashboard_CacheGetCountIsNullButRepoCountFound_ReturnsResponseSuccess()
        {
            // Arrange
            var expectedList = new List<CountByOptionDTO>()
                {
                    new(){ID=Guid.NewGuid(),Count=2,Text="Option 1" },
                    new(){ID=Guid.NewGuid(),Count=4,Text="Option 2" }
                };
            var option = new OptionEntity() { ID = Guid.NewGuid(), PollID = Guid.NewGuid() };

            _voteCountCacheMock
                .Setup(cache => cache.GetCount(option.PollID))
                .Returns<CountByOptionDTO?>(null);

            _voteRepositoryMock
                .Setup(repo => repo.VotesByPoll(option.PollID))
                .Returns(expectedList);

            IResponse expectedResponse = new ResponseSuccess(expectedList, 200);

            // Act
            IResponse response = _voteNotificationService.AddVoteToDashboard(option);

            // Assert
            Assert.Equivalent(expectedResponse, response);
            _voteCountCacheMock.Verify(cache => cache.Set(option.PollID, expectedList), Times.Once);

        }
        [Fact]
        public void AddVoteToDashboard_CacheCountFound_ReturnsResponseSuccess()
        {
            // Arrange
            var countExpected1 = new CountByOptionDTO() { ID = Guid.NewGuid(), Count = 2, Text = "Option 1" };
            var countExpected2 = new CountByOptionDTO() { ID = Guid.NewGuid(), Count = 4, Text = "Option 2" };


            var option = new OptionEntity() { ID = countExpected1.ID, PollID = Guid.NewGuid() };

            var listFound = new List<CountByOptionDTO>()
                {
                    countExpected1,
                    countExpected2
                };

            _voteCountCacheMock
                .Setup(cache => cache.GetCount(option.PollID))
                .Returns(listFound);

            countExpected2.Count++;

            var expectedList = new List<CountByOptionDTO>()
            {
                countExpected1,
                countExpected2
            };
            IResponse expectedResponse = new ResponseSuccess(expectedList, 200);

            // Act
            IResponse response = _voteNotificationService.AddVoteToDashboard(option);

            // Assert
            Assert.Equivalent(expectedResponse, response);
            _voteCountCacheMock.Verify(cache => cache.Set(option.PollID, expectedList), Times.Once);

        }

        [Fact]
        public void GetCountOfVotes_UnexpectedError_ReturnsInternalError()
        {
            // Arrange
            var pollID = Guid.NewGuid();
            _voteCountCacheMock
                .Setup(cache => cache.GetCount(pollID))
                .Throws(new Exception());
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);
            // Act
            IResponse response = _voteNotificationService.GetCountOfVotes(pollID);

            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void GetCountOfVotes_CacheGetCountNullAndRepoVotesByPollNull_ReturnsNotFound()
        {
            // Arrange
            var pollID = Guid.NewGuid();
            _voteCountCacheMock
                .Setup(cache => cache.GetCount(pollID))
                .Returns<List<CountByOptionDTO>?>(null);

            _voteRepositoryMock
                .Setup(repo => repo.VotesByPoll(pollID))
                .Returns<List<CountByOptionDTO>?>(null);

            IResponse expectedResponse = new ResponseFailure("Count was not found", 404);

            // Act
            IResponse response = _voteNotificationService.GetCountOfVotes(pollID);

            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void GetCountOfVotes_CacheGetCountNullAndRepoVotesByPollNotFound_ReturnsNotFound()
        {
            // Arrange
            var pollID = Guid.NewGuid();
            _voteCountCacheMock
                .Setup(cache => cache.GetCount(pollID))
                .Returns<List<CountByOptionDTO>?>(null);

            _voteRepositoryMock
                .Setup(repo => repo.VotesByPoll(pollID))
                .Returns(new List<CountByOptionDTO>());

            IResponse expectedResponse = new ResponseFailure("Count was not found", 404);

            // Act
            IResponse response = _voteNotificationService.GetCountOfVotes(pollID);

            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void GetCountOfVotes_CacheGetCountNullAndRepoVotesByPollFound_ReturnsResponseSuccess()
        {
            // Arrange
            var pollID = Guid.NewGuid();
            var expectedList = new List<CountByOptionDTO>()
                {
                    new(){ID = Guid.NewGuid(), Count = 1, Text = "Option 1"},
                    new(){ID = Guid.NewGuid(), Count = 3, Text = "Option 2"},

                };
            _voteCountCacheMock
                .Setup(cache => cache.GetCount(pollID))
                .Returns<List<CountByOptionDTO>?>(null);

            _voteRepositoryMock
                .Setup(repo => repo.VotesByPoll(pollID))
                .Returns(expectedList);

            IResponse expectedResponse = new ResponseSuccess(expectedList, 200);

            // Act
            IResponse response = _voteNotificationService.GetCountOfVotes(pollID);

            // Assert
            Assert.Equivalent(expectedResponse, response);
            _voteCountCacheMock.Verify(cache => cache.Set(pollID, expectedList), Times.Once);
        }
        [Fact]

        public void GetCountOfVotes_CacheGetCountFound_ReturnsResponseSuccess()
        {
            // Arrange
            var pollID = Guid.NewGuid();
            var expectedList = new List<CountByOptionDTO>()
                {
                    new(){ID = Guid.NewGuid(), Count = 1, Text = "Option 1"},
                    new(){ID = Guid.NewGuid(), Count = 3, Text = "Option 2"},

                };
            _voteCountCacheMock
                .Setup(cache => cache.GetCount(pollID))
                .Returns(expectedList);


            IResponse expectedResponse = new ResponseSuccess(expectedList, 200);

            // Act
            IResponse response = _voteNotificationService.GetCountOfVotes(pollID);

            // Assert
            Assert.Equivalent(expectedResponse, response);
            _voteCountCacheMock.Verify(cache => cache.Set(pollID, expectedList), Times.Once);
        }
    }
}