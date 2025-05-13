using AssetPriceAPI.Controllers;
using AssetPriceAPI.Entities;
using AssetPriceAPI.Models;
using AssetPriceAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AssetPriceAPI.Tests.Controllers
{
    public class PriceControllerTests
    {
        private readonly Mock<IPriceService> _mockPriceService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<PriceController>> _mockLogger;
        private readonly PriceController _controller;

        public PriceControllerTests()
        {
            _mockPriceService = new Mock<IPriceService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<PriceController>>();
            _controller = new PriceController(_mockPriceService.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateOrUpdatePrice_ValidInput_ReturnsOk()
        {
            // Arrange
            var dto = new PriceCreateDto(
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateOnly.FromDateTime(DateTime.UtcNow),
                150.25m
            );

            _mockPriceService.Setup(x => x.AddOrUpdatePriceAsync(dto.AssetId, dto.SourceId, dto.PriceDate, dto.PriceValue))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateOrUpdatePrice(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }
        
        [Fact]
        public async Task GetPrices_ValidRequest_ReturnsOkWithPriceDtos()
        {
            // Arrange
            var date = DateOnly.FromDateTime(DateTime.UtcNow);
            var assetId = Guid.NewGuid();
            var sourceId = Guid.NewGuid();

            var prices = new List<Price>
            {
                new Price
                {
                    Id = Guid.NewGuid(),
                    AssetId = assetId,
                    SourceId = sourceId,
                    PriceDate = date,
                    PriceValue = 123.45m,
                    LastUpdated = DateTime.UtcNow,
                    Asset = new Asset { Id = assetId, Name = "Mock Asset" },
                    Source = new Source { Id = sourceId, Name = "Mock Source" }
                }
            };

            var priceDtos = new List<PriceReadDto>
            {
                new(
                    prices[0].Id,
                    prices[0].AssetId,
                    prices[0].Asset.Name,
                    prices[0].SourceId,
                    prices[0].Source.Name,
                    prices[0].PriceDate,
                    prices[0].PriceValue,
                    prices[0].LastUpdated
                )
            };

            _mockPriceService.Setup(s => s.GetPricesAsync(It.IsAny<List<Guid>>(), date, sourceId))
                .ReturnsAsync(prices);

            _mockMapper.Setup(m => m.Map<List<PriceReadDto>>(It.IsAny<List<Price>>()))
                .Returns(priceDtos);

            // Act
            var result = await _controller.GetPrices(date, new List<Guid> { assetId }, sourceId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDtos = Assert.IsAssignableFrom<List<PriceReadDto>>(okResult.Value);
            Assert.Single(returnedDtos);
            Assert.Equal(prices[0].PriceValue, returnedDtos[0].PriceValue);
        }
    }
}
