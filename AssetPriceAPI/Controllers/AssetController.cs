using AssetPriceAPI.Entities;
using AssetPriceAPI.Models;
using AssetPriceAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AssetPriceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssetController : ControllerBase
{
    private readonly IAssetService _assetService;
    private readonly IMapper _mapper;

    public AssetController(IAssetService assetService, IMapper mapper)
    {
        _assetService = assetService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssetReadDto>>> GetAll()
    {
        var assets = await _assetService.GetAllAssetsAsync();
        return Ok(_mapper.Map<IEnumerable<AssetReadDto>>(assets));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AssetReadDto>> GetById(Guid id)
    {
        var asset = await _assetService.GetAssetByIdAsync(id);
        if (asset == null) return NotFound();

        return Ok(_mapper.Map<AssetReadDto>(asset));
    }

    [HttpGet("symbol/{symbol}")]
    public async Task<ActionResult<AssetReadDto>> GetBySymbol(string symbol)
    {
        var asset = await _assetService.GetAssetBySymbolAsync(symbol);
        if (asset == null) return NotFound();

        return Ok(_mapper.Map<AssetReadDto>(asset));
    }

    [HttpGet("isin/{isin}")]
    public async Task<ActionResult<AssetReadDto>> GetByIsin(string isin)
    {
        var asset = await _assetService.GetAssetByIsinAsync(isin);
        if (asset == null) return NotFound();

        return Ok(_mapper.Map<AssetReadDto>(asset));
    }

    [HttpPost]
    public async Task<ActionResult<AssetReadDto>> Create([FromBody] AssetCreateDto dto)
    {
        var asset = _mapper.Map<Asset>(dto);
        var created = await _assetService.CreateAssetAsync(asset);
        var readDto = _mapper.Map<AssetReadDto>(created);

        return CreatedAtAction(nameof(GetById), new { id = readDto.Id }, readDto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AssetReadDto>> Update(Guid id, [FromBody] AssetCreateDto dto)
    {
        var updatedAsset = new Asset
        {
            Id = id,
            Name = dto.Name,
            Symbol = dto.Symbol,
            Isin = dto.Isin
        };

        var result = await _assetService.UpdateAssetAsync(updatedAsset);
        if (result == null) return NotFound();

        return Ok(_mapper.Map<AssetReadDto>(result));
    }
}