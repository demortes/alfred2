using AlfredBackend.Models;

namespace AlfredBackend.Services
{
    public interface IBotSettingsService
    {
        Task<BotSettings> GetSettingsAsync(string twitchUserId);
        Task<BotSettings> EnsureSettingsExistAsync(string twitchUserId);
        Task<bool> UpdateComponentStateAsync(string twitchUserId, string componentName, bool enabled);
        Task<bool> UpdateTimeoutAsync(string twitchUserId, string timeoutName, int valueSeconds);
        Task SetConnectionStatusAsync(string twitchUserId, string status, string? channelName = null);
        Task<string> GetConnectionStatusAsync(string twitchUserId);
    }
}
