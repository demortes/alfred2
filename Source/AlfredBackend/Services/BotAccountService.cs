using Microsoft.EntityFrameworkCore;
using AlfredBackend.Data;
using AlfredBackend.Models;

namespace AlfredBackend.Services
{
    public class BotAccountService : IBotAccountService
    {
        private readonly AlfredDbContext _context;
        private readonly IAuditLogService _auditLog;
        
        public BotAccountService(AlfredDbContext context, IAuditLogService auditLog)
        {
            _context = context;
            _auditLog = auditLog;
        }
        
        public async Task<BotAccount?> GetBotAccountAsync(int id)
        {
            return await _context.BotAccounts
                .Include(b => b.Owner)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
        
        public async Task<List<BotAccount>> GetUserBotAccountsAsync(string userId)
        {
            return await _context.BotAccounts
                .Where(b => b.OwnerTwitchUserId == userId)
                .ToListAsync();
        }
        
        public async Task<BotAccount> CreateBotAccountAsync(BotAccount botAccount)
        {
            botAccount.CreatedAt = DateTime.UtcNow;
            botAccount.LastTokenRefresh = DateTime.UtcNow;
            
            _context.BotAccounts.Add(botAccount);
            await _context.SaveChangesAsync();
            
            // Log to audit
            await _auditLog.LogAsync(new AuditLogDocument
            {
                UserId = botAccount.OwnerTwitchUserId,
                Action = "BotAccountCreated",
                EntityType = "BotAccount",
                EntityName = botAccount.BotUsername,
                NewValue = botAccount.Id.ToString()
            });
            
            return botAccount;
        }
        
        public async Task<bool> DeleteBotAccountAsync(int id, string userId)
        {
            var botAccount = await _context.BotAccounts
                .FirstOrDefaultAsync(b => b.Id == id && b.OwnerTwitchUserId == userId);
            
            if (botAccount == null)
                return false;
            
            _context.BotAccounts.Remove(botAccount);
            await _context.SaveChangesAsync();
            
            // Log to audit
            await _auditLog.LogAsync(new AuditLogDocument
            {
                UserId = userId,
                Action = "BotAccountDeleted",
                EntityType = "BotAccount",
                EntityName = botAccount.BotUsername,
                OldValue = id.ToString()
            });
            
            return true;
        }
        
        public async Task<bool> RefreshTokenAsync(int id, string accessToken, string refreshToken, DateTime expiresAt)
        {
            var botAccount = await _context.BotAccounts.FindAsync(id);
            if (botAccount == null)
                return false;
            
            botAccount.AccessToken = accessToken;
            botAccount.RefreshToken = refreshToken;
            botAccount.TokenExpiresAt = expiresAt;
            botAccount.LastTokenRefresh = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            // Log to audit
            await _auditLog.LogAsync(new AuditLogDocument
            {
                UserId = botAccount.OwnerTwitchUserId,
                Action = "BotTokenRefreshed",
                EntityType = "BotAccount",
                EntityName = botAccount.BotUsername
            });
            
            return true;
        }
        
        public async Task<BotAccount?> ClaimAvailableBotAccountAsync()
        {
            // Get an active bot account not currently in use
            // This is a simplified version - production would need distributed locking
            return await _context.BotAccounts
                .Where(b => b.IsActive)
                .FirstOrDefaultAsync();
        }
        
        public async Task ReleaseBotAccountAsync(int id)
        {
            // Mark bot as available again
            // Simplified - production needs proper state management
            await Task.CompletedTask;
        }
    }
}
