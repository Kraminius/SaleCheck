using Cronos;
using SaleCheck.Model.Utility;
using SaleCheck.Repositories.Interfaces;

public class ScheduledTaskService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly CronExpression _cronExpression;
    private readonly TimeZoneInfo _timeZoneInfo;

    public ScheduledTaskService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _cronExpression = CronExpression.Parse("0 4 * * *");
        _timeZoneInfo = TimeZoneInfo.Local; 
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var nextRun = _cronExpression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
            if (nextRun.HasValue)
            {
                var delay = nextRun.Value - DateTimeOffset.Now;
                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, stoppingToken);
                }

                await RunScheduledTask(stoppingToken);
            }
        }
    }

    public async Task ResetScrapeDate()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var websiteRepository = scope.ServiceProvider.GetRequiredService<IWebsiteRepository>();

            try
            {
                var websites = await websiteRepository.GetAllWebsitesAsync();

                foreach (var website in websites)
                {
                    Console.WriteLine("Resat time of website: {0}", website.WebsiteName);
                    website.LastScrapedDate = DateTime.UtcNow.AddDays(-1);
                    
                    await websiteRepository.UpdateWebsiteAsync(website.WebsiteId, website);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        
    } 

    public async Task RunScheduledTask(CancellationToken cancellationToken)
{
    using (var scope = _serviceProvider.CreateScope())
    {
        // Resolve DataFactory
        var dataFactory = scope.ServiceProvider.GetRequiredService<DataFactory>();
        if (dataFactory == null)
        {
            Console.WriteLine("Error: DataFactory could not be resolved.");
            return;
        }

        // Resolve IWebsiteRepository
        var websiteRepository = scope.ServiceProvider.GetRequiredService<IWebsiteRepository>();
        if (websiteRepository == null)
        {
            Console.WriteLine("Error: IWebsiteRepository could not be resolved.");
            return;
        }

        try
        {
            Console.WriteLine("Running scheduled task: Scrape all websites");

            // Fetch all websites
            var websites = await websiteRepository.GetAllWebsitesAsync();
            if (websites == null)
            {
                Console.WriteLine("Error: Websites could not be retrieved.");
                return;
            }
            Console.WriteLine("Running scheduled task: Scrape websites on: " + websites.Count() + " websites");

            var today = DateTime.UtcNow.Date;

            // Iterate over each website
            foreach (var website in websites)
            {
                if (website.LastScrapedDate.HasValue && website.LastScrapedDate.Value >= DateTime.UtcNow.Date)
                {
                    Console.WriteLine($"Skipping website {website.WebsiteId} as it has already been scraped today.");
                    continue;
                }

                Console.WriteLine($"Updating website: {website.WebsiteId}");
                await dataFactory.UpdateWebsiteByCheckingExistingSubsites(website.WebsiteId, true);

                // Update LastScrapedDate after successful scraping
                website.LastScrapedDate = today;
                await websiteRepository.UpdateWebsiteAsync(website.WebsiteId, website);
            }

        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occurred in RunScheduledTask: {e.Message}");
        }
    }
}



}