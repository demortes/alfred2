using Microsoft.EntityFrameworkCore;
using AlfredBackend.Data;
using AlfredBackend.Models;

namespace AlfredBackend.Services
{
    public class BotSettingsService : IBotSettingsService
    {
        private readonly AlfredDbContext _context;
        private readonly IAuditLogService _auditLog;
        
        public BotSettingsService(AlfredDbContext context, IAuditLogService auditLog)
        {
            _context = context;
            _auditLog = auditLog;
        }
        
        public async Task<BotSettings> GetSettingsAsync(string twitchUserId)
        {
            var settings = await _context.BotSettings
                .Include(s => s.Components)
                .Include(s => s.Timeouts)
                .FirstOrDefaultAsync(s => s.TwitchUserId == twitchUserId);
            
            if (settings == null)
            {
                settings = await EnsureSettingsExistAsync(twitchUserId);
            }
            
            return settings;
        }
        
        public async Task<BotSettings> EnsureSettingsExistAsync(string twitchUserId)
        {
            var existingSettings = await _context.BotSettings
                .FirstOrDefaultAsync(s => s.TwitchUserId == twitchUserId);
            
            if (existingSettings != null)
                return existingSettings;
            
            // Create default settings
            var defaultSettings = new BotSettings
            {
                TwitchUserId = twitchUserId,
                ConnectionStatus = "Disconnected",
                Components = new List<ComponentSetting>
                {
                    new() { Name = "Commands", Description = "Enable bot commands in chat", Enabled = true },
                    new() { Name = "Moderation", Description = "Automated moderation features", Enabled = true },
                    new() { Name = "EventSub", Description = "Twitch EventSub notifications", Enabled = false },
                    new() { Name = "AutoResponses", Description = "Automatic chat responses", Enabled = true }
                },
                Timeouts = new List<TimeoutSetting>
                {
                    new() { Name = "CommandCooldown", Description = "Cooldown between commands", ValueSeconds = 3 },
                    new() { Name = "ReconnectDelay", Description = "Delay before reconnecting", ValueSeconds = 5 },
                    new() { Name = "MessageDelay", Description = "Delay between messages", ValueSeconds = 1 }
                }
            };
            
            _context.BotSettings.Add(defaultSettings);
            await _context.SaveChangesAsync();
            
            // Log creation
            await _auditLog.LogAsync(new AuditLogDocument
            {
                UserId = twitchUserId,
                Action = "SettingsCreated",
                EntityType = "BotSettings",
                EntityName = "Default",
                NewValue = "Initialized with defaults"
            });
            
            return defaultSettings;
        }
        
        public async Task<bool> UpdateComponentStateAsync(string twitchUserId, string componentName, bool enabled)
        {
            var settings = await GetSettingsAsync(twitchUserId);
            var component = settings.Components.FirstOrDefault(c => c.Name == componentName);
            
            if (component == null)
                return false;
            
            var oldValue = component.Enabled.ToString();
            component.Enabled = enabled;
            
            await _context.SaveChangesAsync();
            
            // Log change
            await _auditLog.LogAsync(new AuditLogDocument
            {
                UserId = twitchUserId,
                Action = "ComponentToggled",
                EntityType = "ComponentSetting",
                EntityName = componentName,
                OldValue = oldValue,
                NewValue = enabled.ToString()
            });
            
            return true;
        }
        
        public async Task<bool> UpdateTimeoutAsync(string twitchUserId, string timeoutName, int valueSeconds)
        {
            var settings = await GetSettingsAsync(twitchUserId);
            var timeout = settings.Timeouts.FirstOrDefault(t => t.Name == timeoutName);
            
            if (timeout == null)
                return false;
            
            var oldValue = timeout.ValueSeconds.ToString();
            timeout.ValueSeconds = valueSeconds;
            
            await _context.SaveChangesAsync();
            
            // Log change
            await _auditLog.LogAsync(new AuditLogDocument
            {
                UserId = twitchUserId,
                Action = "TimeoutUpdated",
                EntityType = "TimeoutSetting",
                EntityName = timeoutName,
                OldValue = oldValue,
                NewValue = valueSeconds.ToString()
            });
            
            return true;
        }
        
        public async Task SetConnectionStatusAsync(string twitchUserId, string status, string? channelName = null)
        {
            var settings = await GetSettingsAsync(twitchUserId);
            
            var oldStatus = settings.ConnectionStatus;
            settings.ConnectionStatus = status;
            settings.ChannelName = channelName;
            
            await _context.SaveChangesAsync();
            
            // Log change
            await _auditLog.LogAsync(new AuditLogDocument
            {
                UserId = twitchUserId,
                Action = status == "Connected" ? "ChannelJoined" : "ChannelLeft",
                EntityType = "ChannelConnection",
                EntityName = channelName ?? "Unknown",
                OldValue = oldStatus,
                NewValue = status
            });
        }
        
        public async Task<string> GetConnectionStatusAsync(string twitchUserId)
        {
            var settings = await GetSettingsAsync(twitchUserId);
            return settings.ConnectionStatus;
        }
    }
}
