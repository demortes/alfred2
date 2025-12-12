using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlfredBackend.Models
{
    public class BotSettings
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string TwitchUserId { get; set; } = string.Empty; // FK to TwitchUser
        
        public List<ComponentSetting> Components { get; set; } = new();
        public List<TimeoutSetting> Timeouts { get; set; } = new();
        public string ConnectionStatus { get; set; } = "Disconnected";
        public string? ChannelName { get; set; }
        
        // Navigation
        [ForeignKey(nameof(TwitchUserId))]
        public TwitchUser User { get; set; } = null!;
    }
}
