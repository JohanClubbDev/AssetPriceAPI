using AssetPriceAPI.Entities;
using AssetPriceAPI.Models;
using AssetPriceAPI.Repositories;
using AssetPriceAPI.Services;
using Moq;
using Xunit;

namespace AssetPriceAPI.Tests.Services;

public class PriceServiceTests
{
    private readonly Mock<IPriceRepository> _repoMock;
    private readonly PriceService _service;

    public PriceServiceTests()
    {
        _repoMock = new Mock<IPriceRepository>();
        _service = new PriceService(_repoMock.Object);
    }

    [Fact]
    public async Task AddOrUpdatePriceAsync_ShouldAddNewPriceAndHistory_WhenNoneExists()
    {
        // Arrange
        var assetId = Guid.NewGuid();
        var sourceId = Guid.NewGuid();
        var date = new DateOnly(2024, 1, 1);
        var value = 100m;

        _repoMock.Setup(r => r.GetByAssetSourceDateAsync(assetId, sourceId, date))
            .ReturnsAsync((Price?)null);

        _repoMock.Setup(r => r.GetCurrentHistoryEntryAsync(assetId, sourceId, date))
            .ReturnsAsync((PriceHistory?)null);

        // Act
        await _service.AddOrUpdatePriceAsync(assetId, sourceId, date, value);

        // Assert
        _repoMock.Verify(r => r.AddPriceAsync(It.Is<Price>(p =>
            p.AssetId == assetId &&
            p.SourceId == sourceId &&
            p.PriceDate == date &&
            p.PriceValue == value
        )), Times.Once);

        _repoMock.Verify(r => r.AddPriceHistoryAsync(It.Is<PriceHistory>(h =>
            h.AssetId == assetId &&
            h.SourceId == sourceId &&
            h.PriceDate == date &&
            h.PriceValue == value &&
            h.ValidTo == null
        )), Times.Once);

        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddOrUpdatePriceAsync_ShouldUpdateExistingPriceAndHistory()
    {
        // Arrange
        var assetId = Guid.NewGuid();
        var sourceId = Guid.NewGuid();
        var date = new DateOnly(2024, 1, 1);
        var yesterday = DateTime.UtcNow.AddDays(-1);
        var existingPrice = new Price
        {
            Id = Guid.NewGuid(),
            AssetId = assetId,
            SourceId = sourceId,
            PriceDate = date,
            PriceValue = 80m,
            LastUpdated = yesterday
        };

        var existingHistory = new PriceHistory
        {
            Id = Guid.NewGuid(),
            AssetId = assetId,
            SourceId = sourceId,
            PriceDate = date,
            PriceValue = 80m,
            ValidFrom = yesterday,
            ValidTo = null
        };

        _repoMock.Setup(r => r.GetByAssetSourceDateAsync(assetId, sourceId, date))
            .ReturnsAsync(existingPrice);

        _repoMock.Setup(r => r.GetCurrentHistoryEntryAsync(assetId, sourceId, date))
            .ReturnsAsync(existingHistory);

        // Act
        await _service.AddOrUpdatePriceAsync(assetId, sourceId, date, 100m);

        // Assert
        _repoMock.Verify(r => r.UpdatePriceAsync(It.Is<Price>(p =>
            p.PriceValue == 100m &&
            p.LastUpdated > yesterday
        )), Times.Once);

        _repoMock.Verify(r => r.UpdatePriceHistoryAsync(It.Is<PriceHistory>(h =>
            h.ValidTo != null
        )), Times.Once);

        _repoMock.Verify(r => r.AddPriceHistoryAsync(It.Is<PriceHistory>(h =>
            h.PriceValue == 100m &&
            h.ValidTo == null
        )), Times.Once);

        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPricesAsync_ReturnsCorrectPrices()
    {
        // Arrange
        var assetId = Guid.NewGuid();
        var sourceId = Guid.NewGuid();
        var date = new DateOnly(2024, 1, 1);

        var prices = new List<Price>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AssetId = assetId,
                SourceId = sourceId,
                PriceDate = date,
                PriceValue = 150m,
                LastUpdated = DateTime.UtcNow,
                Asset = new Asset { Id = assetId, Name = "Asset A", Symbol = "AAA", Isin = "ISINA" },
                Source = new Source { Id = sourceId, Name = "Source A" }
            }
        };

        _repoMock.Setup(r => r.GetByAssetIdsAndDateAsync(It.IsAny<List<Guid>>(), date, sourceId))
            .ReturnsAsync(prices);

        // Act
        var result = await _service.GetPricesAsync(new List<Guid> { assetId }, date, sourceId);

        // Assert
        Assert.Single(result);
        var dto = result.First();
        Assert.Equal(assetId, dto.AssetId);
        Assert.Equal(sourceId, dto.SourceId);
        Assert.Equal(150m, dto.PriceValue);
    }
}
