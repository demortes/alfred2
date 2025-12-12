using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AlfredBackend.Services;

namespace AlfredBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChannelController : ControllerBase
    {
        private readonly IBotSettingsService _settingsService;
        private readonly ILogger<ChannelController> _logger;

        public ChannelController(IBotSettingsService settingsService, ILogger<ChannelController> logger)
        {
            _settingsService = settingsService;
            _logger = logger;
        }

        private string GetTwitchUserId()
        {
            return User.FindFirst("sub")?.Value 
                ?? User.Identity?.Name 
                ?? throw new UnauthorizedAccessException("User ID not found");
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinChannel()
        {
            try
            {
                var userId = GetTwitchUserId();
                var channelName = User.Identity?.Name ?? "Unknown";
                
                await _settingsService.SetConnectionStatusAsync(userId, "Connected", channelName);
                _logger.LogInformation($"Bot joined channel '{channelName}' for user {userId}");
                
                return Ok(new { message = $"Successfully joined channel '{channelName}'", status = "Connected", channelName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining channel");
                return StatusCode(500, "An error occurred while joining the channel");
            }
        }

        [HttpPost("leave")]
        public async Task<IActionResult> LeaveChannel()
        {
            try
            {
                var userId = GetTwitchUserId();
                
                await _settingsService.SetConnectionStatusAsync(userId, "Disconnected");
                _logger.LogInformation($"Bot left channel for user {userId}");
                
                return Ok(new { message = "Successfully left channel", status = "Disconnected" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving channel");
                return StatusCode(500, "An error occurred while leaving the channel");
            }
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            try
            {
                var userId = GetTwitchUserId();
                var status = await _settingsService.GetConnectionStatusAsync(userId);
                return Ok(new { status });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving connection status");
                return StatusCode(500, "An error occurred while retrieving status");
            }
        }
    }
}