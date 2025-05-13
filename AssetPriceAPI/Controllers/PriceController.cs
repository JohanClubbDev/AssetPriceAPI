using AssetPriceAPI.Models;
using AssetPriceAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssetPriceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceController : ControllerBase
    {
        private readonly IPriceService _priceService;
        private readonly ILogger<PriceController> _logger;

        public PriceController(IPriceService priceService, ILogger<PriceController> logger)
        {
            _priceService = priceService;
            _logger = logger;
        }

        /// <summary>
        /// Add or update price for an asset.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdatePrice([FromBody] PriceCreateDto priceDto)
        {
            if (priceDto == null)
            {
                return BadRequest(new { message = "Invalid price data." });
            }

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
        public async Task<IActionResult> GetPrices([FromQuery] DateOnly date, [FromQuery] List<Guid> assetIds, [FromQuery] Guid? sourceId)
        {
            try
            {
                var prices = await _priceService.GetPricesAsync(assetIds, date, sourceId);
                return Ok(prices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving prices.");
                return StatusCode(500, new { message = "An error occurred while retrieving the prices." });
            }
        }
    }
}
