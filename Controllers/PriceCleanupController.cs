using Microsoft.AspNetCore.Mvc;
using SaleCheck.Utility;

namespace SaleCheck.Controllers;

[ApiController]
[Route("[controller]")]
public class PriceCleanupController : ControllerBase
{
    private readonly PriceCleanupService _priceCleanupService;
    private readonly ILogger<PriceCleanupController> _logger;

    public PriceCleanupController(PriceCleanupService priceCleanupService, ILogger<PriceCleanupController> logger)
    {
        _priceCleanupService = priceCleanupService;
        _logger = logger;
    }

    [HttpPost("cleanup-prices")]
    public async Task<IActionResult> CleanupPrices()
    {
        try
        {
            _logger.LogInformation("API call to cleanup prices started.");
            await _priceCleanupService.CleanUpPricesAsync();
            _logger.LogInformation("API call to cleanup prices completed successfully.");
            return Ok(new { message = "Price cleanup completed successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while cleaning up prices.");
            return StatusCode(500, new { message = "An error occurred during price cleanup." });
        }
    }
}