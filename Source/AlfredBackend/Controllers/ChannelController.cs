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

        /// <summary>
        /// Initializes a new instance of the ChannelController and assigns its logger.
        /// </summary>
        public ChannelController(ILogger<ChannelController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Handles a request to join the caller's channel by resolving the channel from the caller's identity and returning a confirmation message.
        /// </summary>
        /// <returns>An OK (200) response containing a confirmation string that the caller's channel was joined.</returns>
        [HttpPost("join")]
        public IActionResult JoinChannel()
        {
            // In a real application, this would trigger the bot to join a channel.
            // For now, we'll just log the action.
            var channelName = User.Identity.Name; // Example: get user's channel name from claims
            _logger.LogInformation($"Request to join channel '{channelName}' received.");
            return Ok($"Successfully joined channel '{channelName}'.");
        }

        /// <summary>
        /// Processes a request for the current user to leave their channel and returns a confirmation message.
        /// </summary>
        /// <returns>An OkObjectResult containing a confirmation string that includes the current user's channel name.</returns>
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