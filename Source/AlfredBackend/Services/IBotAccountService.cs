using AlfredBackend.Models;

namespace AlfredBackend.Services
{
    public interface IBotAccountService
    {
        Task<BotAccount?> GetBotAccountAsync(int id);
        Task<List<BotAccount>> GetUserBotAccountsAsync(string userId);
        Task<BotAccount> CreateBotAccountAsync(BotAccount botAccount);
        Task<bool> DeleteBotAccountAsync(int id, string userId);
        Task<bool> RefreshTokenAsync(int id, string accessToken, string refreshToken, DateTime expiresAt);
        Task<BotAccount?> ClaimAvailableBotAccountAsync(); // For bot workers
        Task ReleaseBotAccountAsync(int id); // For bot workers
    }
}
