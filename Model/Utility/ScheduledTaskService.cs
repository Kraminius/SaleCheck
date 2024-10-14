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

    private async Task RunScheduledTask(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dataFactory = scope.ServiceProvider.GetRequiredService<DataFactory>();
            try
            {
                Console.WriteLine("Running scheduled task: Scrape vinduesgrossisten");
                await dataFactory.UpdateWebsiteByCheckingExistingSubsites("vinduesgrossisten", true);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Already updated today, skipping...");
                Console.WriteLine(e);
            }
        }
    }
}