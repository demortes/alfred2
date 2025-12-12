using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AlfredBackend.Services;

namespace AlfredBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BotSettingsController : ControllerBase
    {
        private readonly IBotSettingsService _settingsService;
        private readonly ILogger<BotSettingsController> _logger;

        public BotSettingsController(IBotSettingsService settingsService, ILogger<BotSettingsController> logger)
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

        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            try
            {
                var userId = GetTwitchUserId();
                var settings = await _settingsService.GetSettingsAsync(userId);
                return Ok(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bot settings");
                return StatusCode(500, "An error occurred while retrieving settings");
            }
        }

        [HttpPut("components/{componentName}")]
        public async Task<IActionResult> UpdateComponent(string componentName, [FromBody] bool enabled)
        {
            try
            {
                var userId = GetTwitchUserId();
                var success = await _settingsService.UpdateComponentStateAsync(userId, componentName, enabled);
                
                if (!success)
                {
                    return NotFound($"Component '{componentName}' not found");
                }

                _logger.LogInformation($"Component '{componentName}' set to {(enabled ? "enabled" : "disabled")} by {userId}");
                return Ok(new { message = $"Component '{componentName}' updated successfully", enabled });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating component '{componentName}'");
                return StatusCode(500, "An error occurred while updating the component");
            }
        }

        [HttpPut("timeouts/{timeoutName}")]
        public async Task<IActionResult> UpdateTimeout(string timeoutName, [FromBody] int valueSeconds)
        {
            try
            {
                if (valueSeconds < 0 || valueSeconds > 300)
                {
                    return BadRequest("Timeout value must be between 0 and 300 seconds");
                }

                var userId = GetTwitchUserId();
                var success = await _settingsService.UpdateTimeoutAsync(userId, timeoutName, valueSeconds);
                
                if (!success)
                {
                    return NotFound($"Timeout '{timeoutName}' not found");
                }

                _logger.LogInformation($"Timeout '{timeoutName}' set to {valueSeconds} seconds by {userId}");
                return Ok(new { message = $"Timeout '{timeoutName}' updated successfully", valueSeconds });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating timeout '{timeoutName}'");
                return StatusCode(500, "An error occurred while updating the timeout");
            }
        }
    }
}
