using AssetPriceAPI.Controllers;
using AssetPriceAPI.Entities;
using AssetPriceAPI.Models;
using AssetPriceAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AssetPriceAPI.Tests.Controllers;

public class AssetControllerTests
{
    private readonly Mock<IAssetService> _assetServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<AssetController>> _loggerMock;
    private readonly AssetController _controller;

    public AssetControllerTests()
    {
        _assetServiceMock = new Mock<IAssetService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<AssetController>>();

        _controller = new AssetController(
            _assetServiceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithMappedAssets()
    {
        var assets = new List<Asset> { new() { Id = Guid.NewGuid(), Name = "Asset A" } };
        var dtos = new List<AssetReadDto> { new(assets[0].Id, "Asset A", "AAA", "ISIN123") };

        _assetServiceMock.Setup(s => s.GetAllAssetsAsync()).ReturnsAsync(assets);
        _mapperMock.Setup(m => m.Map<IEnumerable<AssetReadDto>>(assets)).Returns(dtos);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(dtos, okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsAsset_WhenExists()
    {
        var id = Guid.NewGuid();
        var asset = new Asset { Id = id, Name = "Asset A" };
        var dto = new AssetReadDto(id, "Asset A", "AAA", "ISIN123");

        _assetServiceMock.Setup(s => s.GetAssetByIdAsync(id)).ReturnsAsync(asset);
        _mapperMock.Setup(m => m.Map<AssetReadDto>(asset)).Returns(dto);

        var result = await _controller.GetById(id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(dto, okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        var id = Guid.NewGuid();
        _assetServiceMock.Setup(s => s.GetAssetByIdAsync(id)).ReturnsAsync((Asset?)null);

        var result = await _controller.GetById(id);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Contains("Asset not found", notFound.Value.ToString());
    }

    [Fact]
    public async Task GetBySymbol_ReturnsAsset_WhenFound()
    {
        var symbol = "AAA";
        var asset = new Asset { Id = Guid.NewGuid(), Name = "Asset A", Symbol = symbol };
        var dto = new AssetReadDto(asset.Id, "Asset A", symbol, "ISIN123");

        _assetServiceMock.Setup(s => s.GetAssetBySymbolAsync(symbol)).ReturnsAsync(asset);
        _mapperMock.Setup(m => m.Map<AssetReadDto>(asset)).Returns(dto);

        var result = await _controller.GetBySymbol(symbol);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(dto, okResult.Value);
    }

    [Fact]
    public async Task GetBySymbol_ReturnsNotFound_WhenMissing()
    {
        _assetServiceMock.Setup(s => s.GetAssetBySymbolAsync("XXX")).ReturnsAsync((Asset?)null);

        var result = await _controller.GetBySymbol("XXX");

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Contains("Asset not found", notFound.Value.ToString());
    }

    [Fact]
    public async Task GetByIsin_ReturnsAsset_WhenFound()
    {
        var isin = "ISIN123";
        var asset = new Asset { Id = Guid.NewGuid(), Name = "Asset A", Isin = isin };
        var dto = new AssetReadDto(asset.Id, "Asset A", "AAA", isin);

        _assetServiceMock.Setup(s => s.GetAssetByIsinAsync(isin)).ReturnsAsync(asset);
        _mapperMock.Setup(m => m.Map<AssetReadDto>(asset)).Returns(dto);

        var result = await _controller.GetByIsin(isin);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(dto, okResult.Value);
    }

    [Fact]
    public async Task GetByIsin_ReturnsNotFound_WhenMissing()
    {
        _assetServiceMock.Setup(s => s.GetAssetByIsinAsync("NOTFOUND")).ReturnsAsync((Asset?)null);

        var result = await _controller.GetByIsin("NOTFOUND");

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Contains("Asset not found", notFound.Value.ToString());
    }

    [Fact]
    public async Task Create_ReturnsCreatedAsset()
    {
        var dto = new AssetCreateDto("Asset B", "BBB", "ISIN456");
        var entity = new Asset { Id = Guid.NewGuid(), Name = dto.Name, Symbol = dto.Symbol, Isin = dto.Isin };
        var readDto = new AssetReadDto(entity.Id, dto.Name, dto.Symbol, dto.Isin);

        _mapperMock.Setup(m => m.Map<Asset>(dto)).Returns(entity);
        _assetServiceMock.Setup(s => s.CreateAssetAsync(entity)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<AssetReadDto>(entity)).Returns(readDto);

        var result = await _controller.Create(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(readDto, created.Value);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenDtoIsNull()
    {
        var result = await _controller.Create(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("Invalid asset data", badRequest.Value.ToString());
    }

    [Fact]
    public async Task Update_ReturnsUpdatedAsset_WhenSuccessful()
    {
        var id = Guid.NewGuid();
        var updateDto = new AssetCreateDto("Updated Asset", "UPD", "ISIN789");
        var updatedEntity = new Asset { Id = id, Name = updateDto.Name, Symbol = updateDto.Symbol, Isin = updateDto.Isin };
        var updatedDto = new AssetReadDto(id, updateDto.Name, updateDto.Symbol, updateDto.Isin);

        _assetServiceMock.Setup(s => s.UpdateAssetAsync(It.Is<Asset>(a => a.Id == id)))
                         .ReturnsAsync(updatedEntity);
        _mapperMock.Setup(m => m.Map<AssetReadDto>(updatedEntity)).Returns(updatedDto);

        var result = await _controller.Update(id, updateDto);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(updatedDto, okResult.Value);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenAssetMissing()
    {
        var id = Guid.NewGuid();
        var updateDto = new AssetCreateDto("Updated Asset", "UPD", "ISIN789");

        _assetServiceMock.Setup(s => s.UpdateAssetAsync(It.Is<Asset>(a => a.Id == id)))
                         .ReturnsAsync((Asset?)null);

        var result = await _controller.Update(id, updateDto);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Contains("Asset not found", notFound.Value.ToString());
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenDtoIsNull()
    {
        var result = await _controller.Update(Guid.NewGuid(), null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("Invalid asset data", badRequest.Value.ToString());
    }
}
