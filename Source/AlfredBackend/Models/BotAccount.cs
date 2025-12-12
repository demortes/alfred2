using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlfredBackend.Models
{
    public class BotAccount
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string OwnerTwitchUserId { get; set; } = string.Empty; // FK to TwitchUser
        
        [Required]
        [MaxLength(100)]
        public string BotTwitchId { get; set; } = string.Empty; // Bot's Twitch ID
        
        [Required]
        [MaxLength(100)]
        public string BotUsername { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? BotDisplayName { get; set; }
        
        // OAuth Tokens (should be encrypted in production)
        [Required]
        public string AccessToken { get; set; } = string.Empty;
        
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
        
        public DateTime TokenExpiresAt { get; set; }
        
        // IRC/WebSocket credentials
        public string? IrcToken { get; set; }
        
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastTokenRefresh { get; set; } = DateTime.UtcNow;
        
        // Navigation
        [ForeignKey(nameof(OwnerTwitchUserId))]
        public TwitchUser Owner { get; set; } = null!;
    }
}
