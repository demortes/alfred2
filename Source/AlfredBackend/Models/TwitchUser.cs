using System.ComponentModel.DataAnnotations;

namespace AlfredBackend.Models
{
    public class TwitchUser
    {
        [Key]
        public string TwitchId { get; set; } = string.Empty; // Twitch User ID - Primary Key
        
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? DisplayName { get; set; }
        
        [MaxLength(255)]
        public string? Email { get; set; }
        
        [MaxLength(500)]
        public string? ProfileImageUrl { get; set; }
        
        public DateTime FirstSeen { get; set; } = DateTime.UtcNow;
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
        public bool IsBanned { get; set; } = false;
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public BotSettings? BotSettings { get; set; }
        public List<BotAccount> BotAccounts { get; set; } = new();
    }
}
