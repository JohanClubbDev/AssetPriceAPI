using AssetPriceAPI.Models;
using AssetPriceAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AssetPriceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SourceController : ControllerBase
{
    private readonly ISourceService _sourceService;
    private readonly ILogger<SourceController> _logger;

    public SourceController(ISourceService sourceService, ILogger<SourceController> logger)
    {
        _sourceService = sourceService;
        _logger = logger;
    }

    /// <summary>
    /// Get all sources.
    /// </summary>
    /// <returns>A list of sources.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllSources()
    {
        try
        {
            var sources = await _sourceService.GetAllSourcesAsync();
            return Ok(sources);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving sources.");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get a source by its ID.
    /// </summary>
    /// <param name="id">The ID of the source to retrieve.</param>
    /// <returns>The requested source or a 404 if not found.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSourceById(Guid id)
    {
        try
        {
            var source = await _sourceService.GetSourceByIdAsync(id);
            if (source == null)
            {
                return NotFound(new { message = "Source not found" });
            }

            return Ok(source);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the source.");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new source.
    /// </summary>
    /// <param name="sourceDto">The data to create the source.</param>
    /// <returns>The created source.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateSource([FromBody] SourceCreateDto sourceDto)
    {
        if (sourceDto == null || string.IsNullOrWhiteSpace(sourceDto.Name))
        {
            return BadRequest(new { message = "Invalid source data" });
        }

        try
        {
            var createdSource = await _sourceService.CreateSourceAsync(sourceDto);
            return CreatedAtAction(nameof(GetSourceById), new { id = createdSource.Id }, createdSource);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the source.");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a source by its ID.
    /// </summary>
    /// <param name="id">The ID of the source to delete.</param>
    /// <returns>Action result indicating success or failure.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSource(Guid id)
    {
        try
        {
            var result = await _sourceService.DeleteSourceAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Source not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting the source.");
            return StatusCode(500, "Internal server error");
        }
    }
}