using AssetPriceAPI.Entities;
using AssetPriceAPI.Models;
using AssetPriceAPI.Repositories;
using AssetPriceAPI.Services;
using AutoMapper;
using Moq;
using Xunit;

namespace AssetPriceAPI.Tests.Services;

public class SourceServiceTests
{
    private readonly Mock<ISourceRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly SourceService _service;

    public SourceServiceTests()
    {
        _repoMock = new Mock<ISourceRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new SourceService(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetSourceByIdAsync_ReturnsMappedDto_WhenFound()
    {
        var id = Guid.NewGuid();
        var source = new Source { Id = id, Name = "Bloomberg" };
        var dto = new SourceReadDto(id, "Bloomberg");

        _repoMock.Setup(r => r.GetSourceByIdAsync(id)).ReturnsAsync(source);
        _mapperMock.Setup(m => m.Map<SourceReadDto>(source)).Returns(dto);

        var result = await _service.GetSourceByIdAsync(id);

        Assert.Equal(dto, result);
    }

    [Fact]
    public async Task GetSourceByIdAsync_ReturnsNull_WhenNotFound()
    {
        var id = Guid.NewGuid();

        _repoMock.Setup(r => r.GetSourceByIdAsync(id)).ReturnsAsync((Source?)null);

        var result = await _service.GetSourceByIdAsync(id);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetSourceByNameAsync_ReturnsMappedDto_WhenFound()
    {
        var name = "Reuters";
        var source = new Source { Id = Guid.NewGuid(), Name = name };
        var dto = new SourceReadDto(source.Id, name);

        _repoMock.Setup(r => r.GetSourceByNameAsync(name)).ReturnsAsync(source);
        _mapperMock.Setup(m => m.Map<SourceReadDto>(source)).Returns(dto);

        var result = await _service.GetSourceByNameAsync(name);

        Assert.Equal(dto, result);
    }

    [Fact]
    public async Task GetSourceByNameAsync_ReturnsNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetSourceByNameAsync("DoesNotExist")).ReturnsAsync((Source?)null);

        var result = await _service.GetSourceByNameAsync("DoesNotExist");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllSourcesAsync_ReturnsMappedDtos()
    {
        var sources = new List<Source>
        {
            new() { Id = Guid.NewGuid(), Name = "S1" },
            new() { Id = Guid.NewGuid(), Name = "S2" }
        };

        var dtos = sources.Select(s => new SourceReadDto(s.Id, s.Name)).ToList();

        _repoMock.Setup(r => r.GetAllSourcesAsync()).ReturnsAsync(sources);
        _mapperMock.Setup(m => m.Map<IEnumerable<SourceReadDto>>(sources)).Returns(dtos);

        var result = await _service.GetAllSourcesAsync();

        Assert.Equal(dtos, result);
    }

    [Fact]
    public async Task CreateSourceAsync_ReturnsMappedDto()
    {
        var createDto = new SourceCreateDto("Yahoo Finance");
        var entity = new Source { Id = Guid.NewGuid(), Name = createDto.Name };
        var readDto = new SourceReadDto(entity.Id, entity.Name);

        _mapperMock.Setup(m => m.Map<Source>(createDto)).Returns(entity);
        _repoMock.Setup(r => r.AddSourceAsync(entity)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<SourceReadDto>(entity)).Returns(readDto);

        var result = await _service.CreateSourceAsync(createDto);

        Assert.Equal(readDto, result);
    }

    [Fact]
    public async Task UpdateSourceAsync_ReturnsMappedDto_WhenExists()
    {
        var id = Guid.NewGuid();
        var existing = new Source { Id = id, Name = "Old" };
        var updated = new Source { Id = id, Name = "New" };

        var updateDto = new SourceCreateDto("New");
        var readDto = new SourceReadDto(id, "New");

        _repoMock.Setup(r => r.GetSourceByIdAsync(id)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.UpdateSourceAsync(existing)).ReturnsAsync(updated);
        _mapperMock.Setup(m => m.Map<SourceReadDto>(updated)).Returns(readDto);

        var result = await _service.UpdateSourceAsync(id, updateDto);

        Assert.Equal(readDto, result);
    }

    [Fact]
    public async Task UpdateSourceAsync_ReturnsNull_WhenNotFound()
    {
        var id = Guid.NewGuid();
        var dto = new SourceCreateDto("Update");

        _repoMock.Setup(r => r.GetSourceByIdAsync(id)).ReturnsAsync((Source?)null);

        var result = await _service.UpdateSourceAsync(id, dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteSourceAsync_CallsRepository_AndReturnsTrue()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.DeleteSourceAsync(id)).ReturnsAsync(true);

        var result = await _service.DeleteSourceAsync(id);

        Assert.True(result);
        _repoMock.Verify(r => r.DeleteSourceAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteSourceAsync_ReturnsFalse_WhenNotFound()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.DeleteSourceAsync(id)).ReturnsAsync(false);

        var result = await _service.DeleteSourceAsync(id);

        Assert.False(result);
    }
}
