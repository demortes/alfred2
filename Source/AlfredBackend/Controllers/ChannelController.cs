using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AlfredBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChannelController : ControllerBase
    {
        private readonly ILogger<ChannelController> _logger;

        public ChannelController(ILogger<ChannelController> logger)
        {
            _logger = logger;
        }

        [HttpPost("join")]
        public IActionResult JoinChannel()
        {
            // In a real application, this would trigger the bot to join a channel.
            // For now, we'll just log the action.
            var channelName = User.Identity.Name; // Example: get user's channel name from claims
            _logger.LogInformation($"Request to join channel '{channelName}' received.");
            return Ok($"Successfully joined channel '{channelName}'.");
        }

        [HttpPost("leave")]
        public IActionResult LeaveChannel()
        {
            // In a real application, this would trigger the bot to leave a channel.
            // For now, we'll just log the action.
            var channelName = User.Identity.Name; // Example: get user's channel name from claims
            _logger.LogInformation($"Request to leave channel '{channelName}' received.");
            return Ok($"Successfully left channel '{channelName}'.");
        }
    }
}