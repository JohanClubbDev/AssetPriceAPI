using AssetPriceAPI.Entities;
using AssetPriceAPI.Repositories;
using AssetPriceAPI.Services;
using Moq;
using Xunit;

namespace AssetPriceAPI.Tests.Services;

public class AssetServiceTests
{
    private readonly Mock<IAssetRepository> _repoMock;
    private readonly AssetService _service;

    public AssetServiceTests()
    {
        _repoMock = new Mock<IAssetRepository>();
        _service = new AssetService(_repoMock.Object);
    }

    [Fact]
    public async Task GetAllAssetsAsync_ReturnsAllAssets()
    {
        // Arrange
        var assets = new List<Asset>
        {
            new() { Id = Guid.NewGuid(), Name = "Asset1", Symbol = "A1", Isin = "ISIN1" },
            new() { Id = Guid.NewGuid(), Name = "Asset2", Symbol = "A2", Isin = "ISIN2" }
        };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(assets);

        // Act
        var result = await _service.GetAllAssetsAsync();

        // Assert
        Assert.Equal(2, result.Count());
        _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAssetByIdAsync_ReturnsAsset_WhenFound()
    {
        var id = Guid.NewGuid();
        var asset = new Asset { Id = id, Name = "Test", Symbol = "TST", Isin = "ISIN123" };
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(asset);

        var result = await _service.GetAssetByIdAsync(id);

        Assert.Equal(asset, result);
        _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetAssetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Asset?)null);

        var result = await _service.GetAssetByIdAsync(id);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAssetBySymbolAsync_ReturnsCorrectAsset()
    {
        var symbol = "XYZ";
        var asset = new Asset { Id = Guid.NewGuid(), Name = "AssetXYZ", Symbol = symbol, Isin = "ISINXYZ" };

        _repoMock.Setup(r => r.GetBySymbolAsync(symbol)).ReturnsAsync(asset);

        var result = await _service.GetAssetBySymbolAsync(symbol);

        Assert.Equal(asset, result);
    }

    [Fact]
    public async Task GetAssetByIsinAsync_ReturnsCorrectAsset()
    {
        var isin = "ISIN789";
        var asset = new Asset { Id = Guid.NewGuid(), Name = "Asset789", Symbol = "789", Isin = isin };

        _repoMock.Setup(r => r.GetByIsinAsync(isin)).ReturnsAsync(asset);

        var result = await _service.GetAssetByIsinAsync(isin);

        Assert.Equal(asset, result);
    }

    [Fact]
    public async Task CreateAssetAsync_AddsAssetAndReturnsIt()
    {
        var asset = new Asset { Id = Guid.NewGuid(), Name = "New Asset", Symbol = "NEW", Isin = "NEWISIN" };

        _repoMock.Setup(r => r.AddAsync(asset)).Returns(Task.CompletedTask);
        _repoMock.Setup(r => r.SaveChangesAsync());

        var result = await _service.CreateAssetAsync(asset);

        Assert.Equal(asset, result);
        _repoMock.Verify(r => r.AddAsync(asset), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAssetAsync_UpdatesExistingAsset_WhenFound()
    {
        var id = Guid.NewGuid();
        var existing = new Asset { Id = id, Name = "Old", Symbol = "OLD", Isin = "OLDISIN" };
        var updated = new Asset { Id = id, Name = "New", Symbol = "NEW", Isin = "NEWISIN" };

        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.SaveChangesAsync());

        var result = await _service.UpdateAssetAsync(updated);

        Assert.Equal("New", result?.Name);
        Assert.Equal("NEW", result?.Symbol);
        Assert.Equal("NEWISIN", result?.Isin);

        _repoMock.Verify(r => r.Update(existing), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAssetAsync_ReturnsNull_WhenNotFound()
    {
        var asset = new Asset { Id = Guid.NewGuid(), Name = "NotFound", Symbol = "NF", Isin = "NFISIN" };
        _repoMock.Setup(r => r.GetByIdAsync(asset.Id)).ReturnsAsync((Asset?)null);

        var result = await _service.UpdateAssetAsync(asset);

        Assert.Null(result);
        _repoMock.Verify(r => r.Update(It.IsAny<Asset>()), Times.Never);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
