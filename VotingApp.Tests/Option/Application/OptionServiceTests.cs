using Microsoft.Extensions.Logging;
using Moq;
using VotingApp.Base.Domain;
using VotingApp.Context;
using VotingApp.Option.Application;
using VotingApp.Option.Domain;
using VotingApp.Option.Domain.DTO;
using VotingApp.Poll.Domain;

namespace VotingApp.Tests.Option.Application
{
    public class OptionServiceTests : IDisposable
    {
        private readonly Mock<IOptionRepository> _optionRepositoryMock;
        private readonly Mock<IPollRepository> _pollRepositoryMock;
        private readonly OptionService _optionService;
        private bool disposedValue;

        public OptionServiceTests()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            _pollRepositoryMock = new Mock<IPollRepository>();
            _optionRepositoryMock = new Mock<IOptionRepository>();
            var logger = new Mock<ILogger<OptionService>>();


            unitOfWorkMock
                .SetupGet(uow => uow.PollRepository)
                .Returns(_pollRepositoryMock.Object);

            unitOfWorkMock
                .SetupGet(uow => uow.OptionRepository)
                .Returns(_optionRepositoryMock.Object);

            _optionService = new OptionService(unitOfWorkMock.Object, logger.Object);
        }

        [Fact]
        public void Create_UnexpectedError_ReturnsInternalError()
        {   // Arrange
            CreateOptionDTO optionDTO = new();
            _pollRepositoryMock.Setup(rep => rep.GetById(optionDTO.PollID))
                .Throws(new Exception());
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);
            // Act
            IResponse response = _optionService.Create(optionDTO);
            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void Create_PollNotFound_Returns()
        {   // Arrange
            CreateOptionDTO optionDTO = new();
            _pollRepositoryMock.Setup(rep => rep.GetById(optionDTO.PollID))
                .Returns<PollEntity?>(null);
            IResponse expectedResponse = new ResponseFailure("Poll was not found", 404);
            // Act
            IResponse response = _optionService.Create(optionDTO);
            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void Create_CurrentUserIDIsDifferent_ReturnsUnauthorizedError()
        {
            // Arrange
            CreateOptionDTO optionDTO = new()
            {
                PollID = Guid.NewGuid(),
                CurrentUserID = Guid.NewGuid(),
            };

            _pollRepositoryMock.Setup(rep => rep.GetById(optionDTO.PollID))
                .Returns(new PollEntity());

            IResponse expectedResponse = new ResponseFailure("Unauthorized", 401);
            // Act
            IResponse response = _optionService.Create(optionDTO);
            // Assert
            Assert.Equivalent(expectedResponse, response);

        }

        [Fact]
        public void Create_IsValid_ReturnsResponseSuccess()
        {
            // Arrange
            Guid userID = Guid.NewGuid();
            Mock<CreateOptionDTO> optionDTO = new();

            PollEntity pollFound = new() { ID = Guid.NewGuid(), UserID = userID };

            OptionEntity optionEntity = new() { ID = Guid.NewGuid(), PollID = pollFound.ID };

            _pollRepositoryMock
                .Setup(rep => rep.GetById(pollFound.ID))
                .Returns(pollFound);

            IResponse expectedResponse = new ResponseSuccess(ResponseOptionDTO.FromOption(optionEntity), 201);

            optionDTO.SetupAllProperties();
            optionDTO.Object.CurrentUserID = userID;
            optionDTO.Setup(dto => dto.ToEntity()).Returns(optionEntity);
            // Act
            IResponse response = _optionService.Create(optionDTO.Object);
            // Assert
            Assert.Equivalent(expectedResponse, response);
            _optionRepositoryMock.Verify(repo => repo.Create(optionEntity), Times.Once);
        }

        [Fact]
        public void Delete_UnexpectedError_ReturnsInternalError()
        {
            // Arrange
            DeleteDTO deleteDTO = new() { ID = Guid.NewGuid() };
            _optionRepositoryMock
            .Setup(repo => repo.GetById(deleteDTO.ID, o => o.Poll))
            .Throws(new Exception());
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);
            // Act
            IResponse response = _optionService.Delete(deleteDTO);
            // Assert   
            Assert.Equivalent(expectedResponse, response);
        }
        [Fact]
        public void Delete_OptionWasNotFound_ReturnsNotFoundError()
        {
            // Arrange
            DeleteDTO deleteDTO = new() { ID = Guid.NewGuid() };
            _optionRepositoryMock
            .Setup(repo => repo.GetById(deleteDTO.ID, o => o.Poll))
            .Returns<OptionEntity?>(null);
            IResponse expectedResponse = new ResponseFailure("Option was not found", 404);
            // Act
            IResponse response = _optionService.Delete(deleteDTO);
            // Assert   
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void Delete_CurrentUserIDIsDifferent_ReturnsUnauthorizedError()
        {
            // Arrange
            DeleteDTO deleteDTO = new()
            {
                ID = Guid.NewGuid(),
                CurrentUserID = Guid.NewGuid()
            };
            OptionEntity optionFound = new()
            {
                ID = deleteDTO.ID,
                Poll = new() { UserID = Guid.NewGuid() }
            };
            _optionRepositoryMock
            .Setup(repo => repo.GetById(deleteDTO.ID, o => o.Poll))
            .Returns(optionFound);
            IResponse expectedResponse = new ResponseFailure("Unauthorized", 401);
            // Act
            IResponse response = _optionService.Delete(deleteDTO);
            // Assert   
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void Delete_IsValid_ReturnsSuccessResponse()
        {
            // Arrange
            DeleteDTO deleteDTO = new()
            {
                ID = Guid.NewGuid(),
                CurrentUserID = Guid.NewGuid()
            };
            OptionEntity optionFound = new()
            {
                ID = deleteDTO.ID,
                Poll = new() { UserID = deleteDTO.CurrentUserID }
            };
            _optionRepositoryMock
            .Setup(repo => repo.GetById(deleteDTO.ID, o => o.Poll))
            .Returns(optionFound);
            IResponse expectedResponse = new ResponseSuccess(ResponseOptionDTO.FromOption(optionFound), 200);
            // Act
            IResponse response = _optionService.Delete(deleteDTO);
            // Assert   
            Assert.Equivalent(expectedResponse, response);
            _optionRepositoryMock.Verify(repo => repo.Delete(deleteDTO.ID), Times.Once);
        }
        [Fact]
        public void Get_UnexpectedError_ReturnsInternalError()
        {
            // Arrange
            OptionFiltersDTO optionDTO = new(1, 10, "", Guid.Empty);
            _optionRepositoryMock
            .Setup(repo => repo.Get(optionDTO.Filters, optionDTO.Pagination))
            .Throws(new Exception());
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);
            // Act
            IResponse response = _optionService.Get(optionDTO);
            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void Get_WithoutData_ReturnsSuccessResponse()
        {
            // Arrange
            OptionFiltersDTO optionDTO = new(1, 10, "", Guid.Empty);
            _optionRepositoryMock
            .Setup(repo => repo.Get(optionDTO.Filters, optionDTO.Pagination))
            .Returns((new(), 1, 0));
            IResponse expectedResponse = new ResponseSuccess(
                new Results<ResponseOptionDTO>()
                {
                    CurrentPage = 1,
                    TotalPages = 0,
                    Data = Enumerable.Empty<ResponseOptionDTO>()
                }
                , 200);
            // Act
            IResponse response = _optionService.Get(optionDTO);
            // Assert
            Assert.Equivalent(expectedResponse, response);
            _optionRepositoryMock.Verify(repo => repo.Get(optionDTO.Filters, optionDTO.Pagination), Times.Once);
        }
        [Fact]
        public void Get_WithData_ReturnsSuccessResponse()
        {
            // Arrange
            OptionFiltersDTO optionDTO = new(1, 10, "", Guid.Empty);
            List<OptionEntity> optionsFound = new()
            {
                new(){ID=Guid.NewGuid(), Text="Testing Option 1", PollID=Guid.NewGuid()},
                new(){ID=Guid.NewGuid(), Text="Testing Option 2", PollID=Guid.NewGuid()},
                new(){ID=Guid.NewGuid(), Text="Testing Option 3", PollID=Guid.NewGuid()},
            };
            _optionRepositoryMock
            .Setup(repo => repo.Get(optionDTO.Filters, optionDTO.Pagination))
                .Returns((optionsFound, 1, 1));

            IResponse expectedResponse = new ResponseSuccess(
                new Results<ResponseOptionDTO>()
                {
                    CurrentPage = 1,
                    TotalPages = 1,
                    Data = optionsFound.Select(ResponseOptionDTO.FromOption)
                }
                , 200);

            // Act
            IResponse response = _optionService.Get(optionDTO);

            // Assert
            Assert.Equivalent(expectedResponse, response);

            _optionRepositoryMock.Verify(repo => repo.Get(optionDTO.Filters, optionDTO.Pagination), Times.Once);
        }

        [Fact]
        public void GetById_UnexpectedError_ReturnsInternalError()
        {
            // Arrange
            GetByIDDTO getDTO = new();
            _optionRepositoryMock
            .Setup(repo => repo.GetById(getDTO.ID))
            .Throws(new Exception());
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);
            // Act
            IResponse response = _optionService.GetById(getDTO);
            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void GetById_OptionNotFound_ReturnsNotFoundError()
        {
            // Arrange
            GetByIDDTO getDTO = new();
            _optionRepositoryMock
            .Setup(repo => repo.GetById(getDTO.ID))
            .Returns<OptionEntity?>(null);
            IResponse expectedResponse = new ResponseFailure("Option was not found", 404);
            // Act
            IResponse response = _optionService.GetById(getDTO);
            // Assert
            Assert.Equivalent(expectedResponse, response);
        }
        [Fact]
        public void GetById_OptionFound_ReturnsResponseSuccess()
        {
            // Arrange
            GetByIDDTO getDTO = new();
            OptionEntity optionFound = new()
            {
                ID = Guid.NewGuid(),
                Text = "Testing Text",
            };
            _optionRepositoryMock
            .Setup(repo => repo.GetById(getDTO.ID))
            .Returns(optionFound);
            IResponse expectedResponse = new ResponseSuccess(ResponseOptionDTO.FromOption(optionFound), 200);
            // Act
            IResponse response = _optionService.GetById(getDTO);
            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void Update_UnexpectedError_ReturnsInternalError()
        {
            // Arrange
            UpdateOptionDTO updateDTO = new();
            _optionRepositoryMock
                .Setup(repo => repo.GetById(updateDTO.ID, o => o.Poll))
                .Throws(new Exception());
            IResponse expectedResponse = new ResponseFailure("Internal Error", 500);
            // Act
            IResponse response = _optionService.Update(updateDTO);
            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void Update_OptionNotFound_ReturnsNotFoundError()
        {
            // Arrange
            UpdateOptionDTO updateDTO = new();
            _optionRepositoryMock
                .Setup(repo => repo.GetById(updateDTO.ID, o => o.Poll))
                .Returns<OptionEntity?>(null);
            IResponse expectedResponse = new ResponseFailure("Option was not found", 404);

            // Act
            IResponse response = _optionService.Update(updateDTO);

            // Assert
            Assert.Equivalent(expectedResponse, response);
        }

        [Fact]
        public void Update_CurrentUserIsDifferent_ReturnsUnauthorizedError()
        {
            // Arrange
            UpdateOptionDTO updateDTO = new() { ID = Guid.NewGuid(), CurrentUserID = Guid.NewGuid() };
            OptionEntity optionFound = new()
            {
                ID = updateDTO.ID,
                Poll = new() { UserID = Guid.NewGuid() }
            };
            _optionRepositoryMock
                .Setup(repo => repo.GetById(updateDTO.ID, o => o.Poll))
                .Returns(optionFound);

            IResponse expectedResponse = new ResponseFailure("Unauthorized", 401);

            // Act
            IResponse response = _optionService.Update(updateDTO);

            // Assert
            Assert.Equivalent(expectedResponse, response);
        }
        [Fact]
        public void Update_IsValid_ReturnsResponseSuccess()
        {
            // Arrange
            UpdateOptionDTO updateDTO = new() { ID = Guid.NewGuid(), CurrentUserID = Guid.NewGuid() };
            OptionEntity optionFound = new()
            {
                ID = updateDTO.ID,
                Poll = new() { UserID = updateDTO.CurrentUserID }
            };
            _optionRepositoryMock
                .Setup(repo => repo.GetById(updateDTO.ID, o => o.Poll))
                .Returns(optionFound);

            IResponse expectedResponse = new ResponseSuccess(ResponseOptionDTO.FromOption(optionFound), 200);

            // Act
            IResponse response = _optionService.Update(updateDTO);

            // Assert
            Assert.Equivalent(expectedResponse, response);
            _optionRepositoryMock.Verify(repo => repo.Update(optionFound), Times.Once);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _optionRepositoryMock.Reset();
                    _pollRepositoryMock.Reset();

                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~OptionServiceTests()
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
