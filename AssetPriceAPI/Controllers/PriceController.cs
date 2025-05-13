using AssetPriceAPI.Models;
using AssetPriceAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetPriceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PriceController : ControllerBase
{
    private readonly IPriceService _priceService;

    public PriceController(IPriceService priceService)
    {
        _priceService = priceService;
    }

    /// <summary>
    /// Add or update price for an asset.
    /// </summary>
    /// <param name="priceDto">Price data to create or update.</param>
    /// <returns>Action result indicating success or failure.</returns>
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
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get prices of one or more assets for a specific date, optionally from a specific source.
    /// </summary>
    /// <param name="date">The date to retrieve prices for.</param>
    /// <param name="assetIds">Optional: The assets to filter by.</param>
    /// <param name="sourceId">Optional: The source to filter by.</param>
    /// <returns>List of prices matching the given criteria.</returns>
    [HttpGet]
    public async Task<IActionResult> GetPrices([FromQuery] DateOnly date, [FromQuery] List<Guid> assetIds,
        [FromQuery] Guid? sourceId)
    {
        try
        {
            var prices = await _priceService.GetPricesAsync(assetIds, date, sourceId);
            return Ok(prices);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}