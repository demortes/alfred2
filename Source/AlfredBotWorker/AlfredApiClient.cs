using System.Net.Http.Json;

namespace AlfredBotWorker;

public class AlfredApiClient(HttpClient httpClient, ILogger<AlfredApiClient> logger)
{
    public async Task<string> GetBackendStatusAsync(CancellationToken cancellationToken)
    {
        try 
        {
            // Just a test endpoint check, typically usually /health or /api/status.
            // Assuming /health is standard Aspire health check
            // Or we can try to call a bot endpoint if it exists.
            // Let's call the public health check endpoint
            var response = await httpClient.GetAsync("/health", cancellationToken);
            return response.IsSuccessStatusCode ? "Online" : $"Offline ({response.StatusCode})";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error reaching backend");
            return "Unreachable";
        }
    }
}
