using AssetPriceAPI.Entities;
using AssetPriceAPI.Models;
using AssetPriceAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssetPriceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetController : ControllerBase
    {
        private readonly IAssetService _assetService;
        private readonly IMapper _mapper;
        private readonly ILogger<AssetController> _logger;

        public AssetController(IAssetService assetService, IMapper mapper, ILogger<AssetController> logger)
        {
            _assetService = assetService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get a list of all assets.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssetReadDto>>> GetAll()
        {
            try
            {
                var assets = await _assetService.GetAllAssetsAsync();
                return Ok(_mapper.Map<IEnumerable<AssetReadDto>>(assets));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all assets.");
                return StatusCode(500, new { message = "An error occurred while retrieving assets." });
            }
        }

        /// <summary>
        /// Get an asset by its ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AssetReadDto>> GetById(Guid id)
        {
            try
            {
                var asset = await _assetService.GetAssetByIdAsync(id);
                if (asset == null) return NotFound(new { message = "Asset not found." });

                return Ok(_mapper.Map<AssetReadDto>(asset));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving asset by ID.");
                return StatusCode(500, new { message = "An error occurred while retrieving the asset." });
            }
        }

        /// <summary>
        /// Get an asset by its Symbol.
        /// </summary>
        [HttpGet("symbol/{symbol}")]
        public async Task<ActionResult<AssetReadDto>> GetBySymbol(string symbol)
        {
            try
            {
                var asset = await _assetService.GetAssetBySymbolAsync(symbol);
                if (asset == null) return NotFound(new { message = "Asset not found." });

                return Ok(_mapper.Map<AssetReadDto>(asset));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving asset by symbol.");
                return StatusCode(500, new { message = "An error occurred while retrieving the asset." });
            }
        }

        /// <summary>
        /// Get an asset by its ISIN.
        /// </summary>
        [HttpGet("isin/{isin}")]
        public async Task<ActionResult<AssetReadDto>> GetByIsin(string isin)
        {
            try
            {
                var asset = await _assetService.GetAssetByIsinAsync(isin);
                if (asset == null) return NotFound(new { message = "Asset not found." });

                return Ok(_mapper.Map<AssetReadDto>(asset));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving asset by ISIN.");
                return StatusCode(500, new { message = "An error occurred while retrieving the asset." });
            }
        }

        /// <summary>
        /// Create a new asset.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AssetReadDto>> Create([FromBody] AssetCreateDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "Invalid asset data." });
            }

            try
            {
                var asset = _mapper.Map<Asset>(dto);
                var created = await _assetService.CreateAssetAsync(asset);
                var readDto = _mapper.Map<AssetReadDto>(created);

                return CreatedAtAction(nameof(GetById), new { id = readDto.Id }, readDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating asset.");
                return StatusCode(500, new { message = "An error occurred while creating the asset." });
            }
        }

        /// <summary>
        /// Update an existing asset.
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<AssetReadDto>> Update(Guid id, [FromBody] AssetCreateDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "Invalid asset data." });
            }

            try
            {
                var updatedAsset = new Asset
                {
                    Id = id,
                    Name = dto.Name,
                    Symbol = dto.Symbol,
                    Isin = dto.Isin
                };

                var result = await _assetService.UpdateAssetAsync(updatedAsset);
                if (result == null) return NotFound(new { message = "Asset not found." });

                return Ok(_mapper.Map<AssetReadDto>(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating asset.");
                return StatusCode(500, new { message = "An error occurred while updating the asset." });
            }
        }
    }
}
