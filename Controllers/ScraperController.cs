using Microsoft.AspNetCore.Mvc;

namespace SaleCheck.Controllers;

[ApiController]
[Route("[controller]")]
public class ScraperController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;

    public ScraperController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [HttpPost("run")]
    public async Task<IActionResult> RunScraper()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scheduledTaskService = scope.ServiceProvider.GetRequiredService<ScheduledTaskService>();
            if (scheduledTaskService == null)
            {
                return StatusCode(500, "ScheduledTaskService could not be resolved.");
            }

            try
            {
                Console.WriteLine("Manually triggering the scraper.");
                await scheduledTaskService.RunScheduledTask(CancellationToken.None);
                return Ok("Scraper run successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occurred while running the scraper: {e.Message}");
                return StatusCode(500, $"Error: {e.Message}");
            }
        }
    }

    [HttpPost("resetscraper")]
    public async Task<IActionResult> ResetScraper()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var scheduledTaskService = scope.ServiceProvider.GetRequiredService<ScheduledTaskService>();
            if (scheduledTaskService == null)
            {
                return StatusCode(500, "ScheduledTaskService could not be resolved.");
            }

            try
            {
                await scheduledTaskService.ResetScrapeDate();
                return Ok("Scraper run successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occurred while running the scraper: {e.Message}");
                return StatusCode(500, $"Error: {e.Message}");
            }
        }
    }
}