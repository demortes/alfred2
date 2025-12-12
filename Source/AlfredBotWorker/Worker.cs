namespace AlfredBotWorker;

public class Worker(AlfredApiClient apiClient, ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            
            var status = await apiClient.GetBackendStatusAsync(stoppingToken);
            logger.LogInformation("Backend Status: {status}", status);

            await Task.Delay(5000, stoppingToken);
        }
    }
}
