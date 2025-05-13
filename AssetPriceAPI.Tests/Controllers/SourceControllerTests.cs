using AssetPriceAPI.Controllers;
using AssetPriceAPI.Models;
using AssetPriceAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AssetPriceAPI.Tests.Controllers
{
    public class SourceControllerTests
    {
        private readonly Mock<ISourceService> _mockService;
        private readonly Mock<ILogger<SourceController>> _mockLogger;
        private readonly SourceController _controller;

        public SourceControllerTests()
        {
            _mockService = new Mock<ISourceService>();
            _mockLogger = new Mock<ILogger<SourceController>>();
            _controller = new SourceController(_mockService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllSources_ReturnsOkWithSources()
        {
            // Arrange
            var sources = new List<SourceReadDto>
            {
                new(Guid.NewGuid(), "Source A"),
                new(Guid.NewGuid(), "Source B")
            };

            _mockService.Setup(s => s.GetAllSourcesAsync()).ReturnsAsync(sources);

            // Act
            var result = await _controller.GetAllSources();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<IEnumerable<SourceReadDto>>(okResult.Value);
            Assert.Equal(2, ((List<SourceReadDto>)returned).Count);
        }

        [Fact]
        public async Task GetSourceById_ExistingId_ReturnsOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new SourceReadDto(id, "Test Source");

            _mockService.Setup(s => s.GetSourceByIdAsync(id)).ReturnsAsync(dto);

            // Act
            var result = await _controller.GetSourceById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<SourceReadDto>(okResult.Value);
            Assert.Equal(id, returned.Id);
        }

        [Fact]
        public async Task GetSourceById_NonexistentId_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetSourceByIdAsync(id)).ReturnsAsync((SourceReadDto?)null);

            // Act
            var result = await _controller.GetSourceById(id);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFound.StatusCode);
        }

        [Fact]
        public async Task CreateSource_ValidDto_ReturnsCreatedAt()
        {
            // Arrange
            var createDto = new SourceCreateDto("New Source");
            var readDto = new SourceReadDto(Guid.NewGuid(), "New Source");

            _mockService.Setup(s => s.CreateSourceAsync(createDto)).ReturnsAsync(readDto);

            // Act
            var result = await _controller.CreateSource(createDto);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            var returned = Assert.IsType<SourceReadDto>(created.Value);
            Assert.Equal("New Source", returned.Name);
        }

        [Fact]
        public async Task CreateSource_InvalidDto_ReturnsBadRequest()
        {
            // Arrange
            var invalidDto = new SourceCreateDto(""); // Name is required

            // Act
            var result = await _controller.CreateSource(invalidDto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public async Task DeleteSource_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteSourceAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteSource(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteSource_NonexistentId_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteSourceAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteSource(id);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFound.StatusCode);
        }
    }
}
