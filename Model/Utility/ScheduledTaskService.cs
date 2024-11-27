using Cronos;
using SaleCheck.Model.Utility;

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

    public async Task RunScheduledTask(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            // Check if DataFactory is resolved
            var dataFactory = scope.ServiceProvider.GetRequiredService<DataFactory>();
            if (dataFactory == null)
            {
                Console.WriteLine("Error: DataFactory could not be resolved.");
                return;
            }

            try
            {
                Console.WriteLine("Running scheduled task: Scrape vinduesgrossisten");
                await dataFactory.UpdateWebsiteByCheckingExistingSubsites("vinduesgrossisten", true);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occurred in RunScheduledTask: {e.Message}");
                throw;
            }
        }
    }

}