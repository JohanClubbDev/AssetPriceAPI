using AssetPriceAPI.Models;
using AssetPriceAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;

namespace AssetPriceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceController : ControllerBase
    {
        private readonly IPriceService _priceService;
        private readonly ILogger<PriceController> _logger;
        private readonly IMapper _mapper;

        public PriceController(IPriceService priceService, IMapper mapper, ILogger<PriceController> logger)
        {
            _priceService = priceService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Add or update price for an asset.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdatePrice([FromBody] PriceCreateDto priceDto)
        {
            try
            {
                await _priceService.AddOrUpdatePriceAsync(priceDto.AssetId, priceDto.SourceId, priceDto.PriceDate, priceDto.PriceValue);
                return Ok(new { message = "Price created/updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating or updating price.");
                return StatusCode(500, new { message = "An error occurred while creating or updating the price." });
            }
        }

        /// <summary>
        /// Get prices of one or more assets for a specific date, optionally from a specific source.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<PriceReadDto>>> GetPrices([FromQuery] DateOnly date, [FromQuery] List<Guid> assetIds, [FromQuery] Guid? sourceId)
        {
            try
            {
                var prices = await _priceService.GetPricesAsync(assetIds, date, sourceId);
                return Ok(_mapper.Map<List<PriceReadDto>>(prices));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving prices.");
                return StatusCode(500, new { message = "An error occurred while retrieving the prices." });
            }
        }
    }
}
